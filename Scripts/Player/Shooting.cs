using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    public int CurrentAmmo;
    [HideInInspector] public bool CanShoot = true, IsReloading;
    [SerializeField] private Inventory Inventory;
    [SerializeField] private ItemScriptableObjects ItemScriptableObject;
    private Animator Animator;
    [SerializeField] AudioClip PistolShot, ShotgunShot, PistolReloadSound, EmptyMag, ShotgunReloadSound;
    private AudioSource AudioSource;
    [SerializeField] private Vector3 Offset;
    [SerializeField] private ParticleSystem MuzzleFlash, BulletWound;
    [SerializeField] private GameObject MuzzleFlashLight;
    private Transform TargetEnemy;
    [SerializeField] private Transform RightHand, LeftHand, Head;
    private PlayerStats PlayerStats;
    [SerializeField] private LayerMask Obstacles;
    [SerializeField] private Image ReloadImage;
    [SerializeField] private Sprite ReloadSpriteKM, ReloadSpriteC;
    public Collider[] NearEnemies;
    [SerializeField] private float EnemyDetectionRadius;
    [SerializeField] private LayerMask EnemyLayer;

    private void Start()
    {
        Animator = Inventory.gameObject.GetComponentInChildren<Animator>();
        AudioSource = GetComponent<AudioSource>();
        PlayerStats = GetComponentInParent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (MuzzleFlash.isPlaying)
        {
            MuzzleFlashLight.SetActive(true);
        }

        else
        {
            MuzzleFlashLight.SetActive(false);
        }

        if (Time.timeScale == 1.0f && !PlayerStats.BeingHit)
        {
            if (Input.GetAxis("Aim") > 0 || Input.GetButton("Aim"))
            {
                LookAtEnemy();

                // if the player is holding the pistol, do the pistol aim animation
                if (ItemScriptableObject.AmmoType == "Pistol Ammo")
                {
                    Animator.SetInteger("State", 3);
                }

                // if the player is holding the Shotgun, do the shotgun aim animation
                else if (ItemScriptableObject.AmmoType == "Shotgun Ammo")
                {
                    Animator.SetInteger("State", 5);
                }

                // If the player is aiming, presses left click and has ammo above 0
                if (Input.GetButtonDown("Shoot"))
                {
                    if (CanShoot && !IsReloading)
                    {
                        StartCoroutine(FireRateDelay());

                        if (CurrentAmmo > 0)
                        {
                            // If the raycast coming from the gun and hits the enemy layer
                            if (Physics.Raycast(transform.position + Offset, transform.forward, out RaycastHit Hit, ItemScriptableObject.MaxGunDistance) && Hit.transform.tag == "Explosive" ||
                                Physics.Raycast(transform.position + Offset, transform.forward, out Hit, ItemScriptableObject.MaxGunDistance) && Hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy") && Hit.transform.GetComponent<Enemies>().CurrentHealth > 0 ||
                                Physics.Raycast(transform.position + Offset, transform.forward, out Hit, ItemScriptableObject.MaxGunDistance) && Hit.transform.gameObject.layer == LayerMask.NameToLayer("Ghost") && Hit.transform.GetComponent<Enemies>().CurrentHealth > 0 ||
                                Physics.Raycast(transform.position + Offset, transform.forward, out Hit, ItemScriptableObject.MaxGunDistance) && Hit.transform.gameObject.layer == LayerMask.NameToLayer("Head") && Hit.transform.GetComponentInParent<Enemies>().CurrentHealth > 0)
                            {
                                // Headshot
                                if (Hit.collider.tag == "Head")
                                {
                                    if (Hit.collider.transform.root.name != "Boss")
                                    {
                                        Debug.Log("Headshot");
                                        Hit.transform.GetComponentInParent<Enemies>().CurrentHealth -= ItemScriptableObject.GunDamage * ItemScriptableObject.HeadShotMultiplier;
                                        Hit.transform.GetComponentInParent<Enemies>().CurrentState = 4;
                                    }
                                   
                                    Instantiate(BulletWound, Hit.point, Quaternion.identity);
                                }

                                else if (Hit.collider.tag == "Explosive")
                                {
                                    Hit.collider.transform.GetComponent<ExplosiveBarrel>().Explode = true;
                                }

                                // Bodyshot
                                else
                                {
                                    if (Hit.collider.name != "Boss")
                                    {
                                        Debug.Log("Bodyshot");
                                        Hit.transform.GetComponent<Enemies>().CurrentHealth -= ItemScriptableObject.GunDamage;
                                        Hit.transform.GetComponent<Enemies>().CurrentState = 4;
                                    }

                                    Instantiate(BulletWound, Hit.point, Quaternion.identity);
                                }
                            }

                            // Subtract ammo count
                            CurrentAmmo--;

                            MuzzleFlash.Play();

                            if (ItemScriptableObject.AmmoType == "Pistol Ammo")
                            {
                                AudioSource.clip = PistolShot;
                            }

                            else if (ItemScriptableObject.AmmoType == "Shotgun Ammo")
                            {
                                AudioSource.clip = ShotgunShot;
                            }
                        }

                        else
                        {
                            AudioSource.clip = EmptyMag;
                        }

                        AudioSource.Play();

                        if (ItemScriptableObject.AmmoType == "Pistol Ammo")
                        {
                            Animator.SetTrigger("Pistol Shoot");
                        }
                        
                        else if (ItemScriptableObject.AmmoType == "Shotgun Ammo")
                        {
                            Animator.SetTrigger("Shotgun Shoot");
                        }
                    }
                }
            }
            
            else
            {
                TargetEnemy = null;
            }

            // If the player has ran out of ammo and R is pressed then reload gun
            if (CurrentAmmo < ItemScriptableObject.MaxAmmo)
            {
                if (Input.GetButtonDown("Reload"))
                {
                    if (!IsReloading)
                    {
                        StartCoroutine(CheckIfCanReload());
                    }
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (!PlayerStats.BeingHit)
        {
            if (Input.GetAxis("Aim") > 0 || Input.GetButton("Aim"))
            {
                if (Input.GetKey(KeyCode.W) || XInputController.current != null && XInputController.current.leftStick.up.isPressed)
                {
                    if (ItemScriptableObject.AmmoType == "Pistol Ammo")
                    {
                        RightHand.localRotation = new Quaternion(RightHand.localRotation.x + ItemScriptableObject.AimOffset, RightHand.localRotation.y, RightHand.localRotation.z, RightHand.localRotation.w);
                        LeftHand.localRotation = new Quaternion(LeftHand.localRotation.x + ItemScriptableObject.AimOffset, LeftHand.localRotation.y, LeftHand.localRotation.z, LeftHand.localRotation.w);
                        Head.localRotation = new Quaternion(Head.localRotation.x - ItemScriptableObject.AimOffset, Head.localRotation.y, Head.localRotation.z, Head.localRotation.w);
                    }

                    else if (ItemScriptableObject.AmmoType == "Shotgun Ammo")
                    {
                        RightHand.localRotation = new Quaternion(RightHand.localRotation.x, RightHand.localRotation.y, RightHand.localRotation.z + ItemScriptableObject.AimOffset, RightHand.localRotation.w);
                        LeftHand.localRotation = new Quaternion(LeftHand.localRotation.x, LeftHand.localRotation.y + ItemScriptableObject.AimOffset, LeftHand.localRotation.z, LeftHand.localRotation.w);
                        Head.localRotation = new Quaternion(Head.localRotation.x - ItemScriptableObject.AimOffset * -1f, Head.localRotation.y, Head.localRotation.z, Head.localRotation.w);
                    }
                }

                else if (Input.GetKeyUp(KeyCode.W) || XInputController.current != null && XInputController.current.leftStick.up.isPressed)
                {
                    if (ItemScriptableObject.AmmoType == "Pistol Ammo")
                    {
                        RightHand.localRotation = new Quaternion(RightHand.localRotation.x - ItemScriptableObject.AimOffset, RightHand.localRotation.y, RightHand.localRotation.z, RightHand.localRotation.w);
                        LeftHand.localRotation = new Quaternion(LeftHand.localRotation.x - ItemScriptableObject.AimOffset, LeftHand.localRotation.y, LeftHand.localRotation.z, LeftHand.localRotation.w);
                        Head.localRotation = new Quaternion(Head.localRotation.x + ItemScriptableObject.AimOffset, Head.localRotation.y, Head.localRotation.z, Head.localRotation.w);
                    }

                    else if (ItemScriptableObject.AmmoType == "Shotgun Ammo")
                    {
                        RightHand.localRotation = new Quaternion(RightHand.localRotation.x, RightHand.localRotation.y - ItemScriptableObject.AimOffset, RightHand.localRotation.z, RightHand.localRotation.w);
                        LeftHand.localRotation = new Quaternion(LeftHand.localRotation.x, LeftHand.localRotation.y - ItemScriptableObject.AimOffset, LeftHand.localRotation.z, LeftHand.localRotation.w);
                        Head.localRotation = new Quaternion(Head.localRotation.x + ItemScriptableObject.AimOffset * -1f, Head.localRotation.y, Head.localRotation.z, Head.localRotation.w);
                    }
                }

                else if (Input.GetKey(KeyCode.S) || XInputController.current != null && XInputController.current.leftStick.down.isPressed)
                {
                    if (ItemScriptableObject.AmmoType == "Pistol Ammo")
                    {
                        RightHand.localRotation = new Quaternion(RightHand.localRotation.x - ItemScriptableObject.AimOffset, RightHand.localRotation.y, RightHand.localRotation.z, RightHand.localRotation.w);
                        LeftHand.localRotation = new Quaternion(LeftHand.localRotation.x - ItemScriptableObject.AimOffset, LeftHand.localRotation.y, LeftHand.localRotation.z, LeftHand.localRotation.w);
                        Head.localRotation = new Quaternion(Head.localRotation.x + ItemScriptableObject.AimOffset, Head.localRotation.y, Head.localRotation.z, Head.localRotation.w);
                    }

                    else if (ItemScriptableObject.AmmoType == "Shotgun Ammo")
                    {
                        RightHand.localRotation = new Quaternion(RightHand.localRotation.x, RightHand.localRotation.y - ItemScriptableObject.AimOffset * ItemScriptableObject.AimMultiplier, RightHand.localRotation.z, RightHand.localRotation.w);
                        LeftHand.localRotation = new Quaternion(LeftHand.localRotation.x, LeftHand.localRotation.y - ItemScriptableObject.AimOffset * ItemScriptableObject.AimMultiplier, LeftHand.localRotation.z, LeftHand.localRotation.w);
                        Head.localRotation = new Quaternion(Head.localRotation.x + ItemScriptableObject.AimOffset * -1f, Head.localRotation.y, Head.localRotation.z, Head.localRotation.w);
                    }
                }

                else if (Input.GetKeyUp(KeyCode.S) || XInputController.current != null && XInputController.current.leftStick.down.isPressed)
                {
                    if (ItemScriptableObject.AmmoType == "Pistol Ammo")
                    {
                        RightHand.localRotation = new Quaternion(RightHand.localRotation.x + ItemScriptableObject.AimOffset, RightHand.localRotation.y, RightHand.localRotation.z, RightHand.localRotation.w);
                        LeftHand.localRotation = new Quaternion(LeftHand.localRotation.x + ItemScriptableObject.AimOffset, LeftHand.localRotation.y, LeftHand.localRotation.z, LeftHand.localRotation.w);
                        Head.localRotation = new Quaternion(Head.localRotation.x - ItemScriptableObject.AimOffset, Head.localRotation.y, Head.localRotation.z, Head.localRotation.w);
                    }

                    else if (ItemScriptableObject.AmmoType == "Shotgun Ammo")
                    {
                        RightHand.localRotation = new Quaternion(RightHand.localRotation.x, RightHand.localRotation.y + ItemScriptableObject.AimOffset * ItemScriptableObject.AimMultiplier, RightHand.localRotation.z, RightHand.localRotation.w);
                        LeftHand.localRotation = new Quaternion(LeftHand.localRotation.x, LeftHand.localRotation.y + ItemScriptableObject.AimOffset * ItemScriptableObject.AimMultiplier, LeftHand.localRotation.z, LeftHand.localRotation.w);
                        Head.localRotation = new Quaternion(Head.localRotation.x - ItemScriptableObject.AimOffset * -1f, Head.localRotation.y, Head.localRotation.z, Head.localRotation.w);
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draws the line that the bullet travels
        Gizmos.DrawRay(transform.position + Offset, transform.forward * ItemScriptableObject.MaxGunDistance);
    }

    private IEnumerator FireRateDelay()
    {
        // Stop player from shooting
        CanShoot = false;
        // Wait given amount of time
        yield return new WaitForSeconds(ItemScriptableObject.FireRate);
        // Enable shooting
        CanShoot = true;
    }

    private IEnumerator CheckIfCanReload()
    {
        IsReloading = true;

        // Look for the correct ammo type to reload with inside the player's inventory array
        for (int Item = 0; Item < Inventory.InventorySlots.Length; Item++)
        {
            if (Inventory.InventorySlots[Item].ItemName == ItemScriptableObject.AmmoType)
            {
                if (CurrentAmmo < ItemScriptableObject.MaxAmmo)
                {
                    if (ItemScriptableObject.AmmoType == "Pistol Ammo")
                    {
                        AudioSource.clip = PistolReloadSound;
                    }

                    else if (ItemScriptableObject.AmmoType == "Shotgun Ammo")
                    {
                        AudioSource.clip = ShotgunReloadSound;
                    }

                    AudioSource.Play();
                    yield return new WaitForSeconds(ItemScriptableObject.ReloadSpeed);
                    Reload(Item, null);
                    yield break;
                }

                // break out of the loop because the player has full ammo in their gun
                else
                {
                    IsReloading = false;
                    yield break;
                }
            }
        }

        IsReloading = false;
    }

    public void Reload(int Item, TextMeshProUGUI InfoText)
    {
        // If the current ammo is less than the max ammo
        if (CurrentAmmo < ItemScriptableObject.MaxAmmo)
        {
            // Calculates the required ammo withought exceeding the max ammo the gun can hold
            int RequiredAmmo = ItemScriptableObject.MaxAmmo - CurrentAmmo;

            Inventory = GetComponentInParent<Inventory>();
            // If the player does not have enough ammo to max out the gun, then use all the ammo they have
            if (Inventory.InventorySlots[Item].Amount < RequiredAmmo)
            {
                RequiredAmmo = Inventory.InventorySlots[Item].Amount;
            }

            // Adds the ammo into the gun
            CurrentAmmo += RequiredAmmo;

            // Takes the ammo out of the inventory
            Inventory.InventorySlots[Item].Amount -= RequiredAmmo;

            if (InfoText != null)
            {
                InfoText.text = "Item: " + Inventory.InventorySlots[Item].ItemName.ToString();
            }
        }
            

        else if (InfoText != null)
        {
            InfoText.text = Inventory.InventorySlots[Item].ItemName.ToString() + " is already full!";
        }

        IsReloading = false;
    }

    private void LookAtEnemy()
    {
        NearEnemies = Physics.OverlapSphere(transform.position, EnemyDetectionRadius, EnemyLayer);

        // for each nearby enemy
        for (int i = 0; i < NearEnemies.Length; i++)
        {
            // if i == the first near enemy or the distance between i and the gun is less than the distnace between the gun and the current target enemy
            if (i == 0 || TargetEnemy != null && Vector3.Distance(transform.position, NearEnemies[i].transform.position) < Vector3.Distance(transform.position, TargetEnemy.position))
            {
                // If the enemy in i is not dead and is hit by a ray cast directed towards their direction
                if (NearEnemies[i] != null && Physics.Raycast(transform.position + Offset, NearEnemies[i].transform.position - transform.position + Offset, out RaycastHit Hit, Mathf.Infinity, Obstacles))
                {
                    if (NearEnemies[i].tag == "Explosive" || NearEnemies[i].gameObject.GetComponent<Enemies>().CurrentState != 2)
                    {
                        // If the raycast hits the the enemy layers withought hitting any other layer (if there is not an object between the enemy and the gun) then set as the target enemy
                        if (Hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy") || Hit.collider.gameObject.layer == LayerMask.NameToLayer("Ghost") || Hit.collider.gameObject.layer == LayerMask.NameToLayer("Head"))
                        {
                            TargetEnemy = NearEnemies[i].transform;
                        }
                    }
                }
            }
        }

        // Make the player look at the enemy so they can shoot it easily
        if (TargetEnemy != null)
        {
            Inventory.transform.LookAt(new Vector3(TargetEnemy.position.x, Inventory.transform.position.y, TargetEnemy.position.z));
        }
    }
}
