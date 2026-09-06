using UnityEngine;

public class Doors : MonoBehaviour
{
    public bool NearDoor, IsLocked;
    private Animator Animator;
    [SerializeField] private LayerMask PlayerLayer;
    [SerializeField] private float BoxcastDistance;
    [SerializeField] Vector2 FOffset, BOffset;
    private Vector3 Origin1, Origin2;
    [SerializeField] private Vector3 CastScale;
    [SerializeField] private Dialogue Dialogue;
    [SerializeField] private string DialogueText;
    [SerializeField] private GameObject FrontRoom, BackRoom;
    public GameObject LockedDoorImage;
    [SerializeField] private CurrentRoom CurrentRoom;
    [HideInInspector] public AudioSource AudioSource;
    public AudioClip UnlockSound, LockedSound, OpenSound;

    private void Start()
    {
        Animator = GetComponentInChildren<Animator>();
        Origin1 = new Vector3(transform.position.x + FOffset.x, transform.position.y, transform.position.z + FOffset.y);
        Origin2 = new Vector3(transform.position.x + BOffset.x, transform.position.y, transform.position.z - BOffset.y);
        AudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (NearDoor)
        {
            // Tell the player that the door is locked
            if (IsLocked)
            {
                // Display that the door is locked on the map when near the door
                LockedDoorImage.SetActive(true);

                if (Input.GetButtonDown("Shoot") && !Dialogue.IsRunning && Time.timeScale == 1.0f)
                {
                    AudioSource.clip = LockedSound;
                    AudioSource.Play();
                    StartCoroutine(Dialogue.SetDialogue(DialogueText));
                }
            }

            else
            {
                // Use box casts to detect the direction the player is accessing the door
                if (Physics.BoxCast(Origin1, CastScale / 2f, transform.forward, Quaternion.identity, BoxcastDistance, PlayerLayer))// front
                {
                    // Mark the entered room as explored on the map
                    FrontRoom.SetActive(true);
                    CurrentRoom.PreviousRoomImage = CurrentRoom.CurrentRoomImage;
                    CurrentRoom.CurrentRoomImage = FrontRoom;
                }

                else if (Physics.BoxCast(Origin2, CastScale / 2f, -transform.forward, Quaternion.identity, BoxcastDistance, PlayerLayer))// back
                {
                    // Mark the entered room as explored on the map
                    BackRoom.SetActive(true);
                    CurrentRoom.PreviousRoomImage = CurrentRoom.CurrentRoomImage;
                    CurrentRoom.CurrentRoomImage = BackRoom;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the player is near the door
        if (other.tag == "Player")
        {
            NearDoor = true;

            // If the door is not locked
            if (!IsLocked)
            {
                // Use box casts to detect the direction the player is accessing the door
                if (Physics.BoxCast(Origin1, CastScale / 2f, transform.forward, Quaternion.identity, BoxcastDistance, PlayerLayer))// front
                {
                    AudioSource.clip = OpenSound;
                    AudioSource.Play();

                    Animator.SetTrigger("Open1");
                }

                else if (Physics.BoxCast(Origin2, CastScale / 2f, -transform.forward, Quaternion.identity, BoxcastDistance, PlayerLayer))// back
                {
                    AudioSource.clip = OpenSound;
                    AudioSource.Play();

                    Animator.SetTrigger("Open2");
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the player is near the door
        if (other.tag == "Player")
        {
            NearDoor = false;

            // If the door is not locked
            if (!IsLocked)
            {
                // Use box casts to detect the direction the player is accessing the door
                if (Physics.BoxCast(Origin1, CastScale / 2f, transform.forward, Quaternion.identity, BoxcastDistance, PlayerLayer))// front
                {
                    if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Door Open"))
                    {
                        AudioSource.clip = OpenSound;
                        AudioSource.Play();

                        Animator.SetTrigger("Close1");
                    }

                    else if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Door Open 2"))
                    {
                        AudioSource.clip = OpenSound;
                        AudioSource.Play();

                        Animator.SetTrigger("Close2");
                    }
                }

                else if (Physics.BoxCast(Origin2, CastScale / 2f, -transform.forward, Quaternion.identity, BoxcastDistance, PlayerLayer))// back
                {
                    if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Door Open"))
                    {
                        AudioSource.clip = OpenSound;
                        AudioSource.Play();

                        Animator.SetTrigger("Close1");
                    }

                    else if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Door Open 2"))
                    {
                        AudioSource.clip = OpenSound;
                        AudioSource.Play();

                        Animator.SetTrigger("Close2");
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(new Vector3(transform.position.x + FOffset.x, transform.position.y, transform.position.z + FOffset.y), CastScale);
        Gizmos.DrawWireCube(new Vector3(transform.position.x + BOffset.x, transform.position.y, transform.position.z - BOffset.y), CastScale);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Vector3(transform.position.x + FOffset.x, transform.position.y, transform.position.z + FOffset.y), transform.forward * BoxcastDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(new Vector3(transform.position.x + BOffset.x, transform.position.y, transform.position.z - BOffset.y), -transform.forward * BoxcastDistance);
    }
}