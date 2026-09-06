using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class Fireplace : MonoBehaviour
{
    private CinemachineCamera FireplaceCamera, DoorCamera;
    [SerializeField] private CinemachineCamera CurrentCamera;
    [SerializeField] private float Timer;
    private ToggleInventoryMenu ToggleInventoryMenu;
    private ObjectInteraction FireplaceInteraction;
    [SerializeField] private Doors FirePlaceDoor;
    [HideInInspector] public bool Light = false;
    private ParticleSystem Fire;
    private AudioSource AudioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FireplaceCamera = GetComponentInChildren<CinemachineCamera>();
        ToggleInventoryMenu = GameObject.Find("Player").GetComponent<ToggleInventoryMenu>();
        FireplaceInteraction = GetComponent<ObjectInteraction>();
        DoorCamera = FirePlaceDoor.GetComponentInChildren<CinemachineCamera>();
        Fire = GetComponentInChildren<ParticleSystem>();
        AudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Light)
        {
            StartCoroutine(LightFirePlace());
        }
    }

    private IEnumerator LightFirePlace()
    {
        Light = false;

        // Unlock the foor if the player is near the fireplace and uses the lighter
        if (FireplaceInteraction.Colliding)
        {
            ToggleInventoryMenu.LoadInventory(false);
            Time.timeScale = 0.0f;
            FireplaceCamera.Prioritize();
            yield return new WaitForSecondsRealtime(Timer);
            AudioSource.Play();
            Fire.Play();
            Fire.GetComponent<AudioSource>().Play();
            yield return new WaitForSecondsRealtime(Timer);
            DoorCamera.Prioritize();
            FirePlaceDoor.IsLocked = false;
            FirePlaceDoor.AudioSource.clip = FirePlaceDoor.UnlockSound;
            FirePlaceDoor.AudioSource.Play();
            yield return new WaitForSecondsRealtime(Timer);
            CurrentCamera.Prioritize();
            Time.timeScale = 1.0f;
        }

        else
        {
            print("Cannot use at this time");
        }
    }
}
