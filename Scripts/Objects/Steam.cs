using System.Collections;
using UnityEngine;

public class Steam : MonoBehaviour
{
    [SerializeField] private Vector3 Force;
    [SerializeField] private int Damage;
    private GameObject Player;
    [SerializeField] private float PushTimer;
    private Animator PlayerAnimator;
    private PlayerStats PlayerStats;
    private Rigidbody PlayerRigidbody;
    private PlayerMovement PlayerMovement;
    [HideInInspector] public AudioSource AudioSource;

    private void Start()
    {
        Player = GameObject.Find("Player");
        PlayerAnimator = Player.GetComponentInChildren<Animator>();
        PlayerStats = Player.GetComponent<PlayerStats>();
        PlayerRigidbody = Player.GetComponent<Rigidbody>();
        PlayerMovement = Player.GetComponent<PlayerMovement>();
        AudioSource = Player.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the player hits the steam
        if (other.tag == "Player")
        {
            // Subtract from the player health
            PlayerStats.CurrentHealth -= Damage;

            StartCoroutine(PushPlayer());

            if (PlayerStats.CurrentHealth > 0)
            {
                // Play the player hit animation
                PlayerAnimator.SetTrigger("Hit");
                PlayerAnimator.SetInteger("State", -1);
                PlayerStats.BeingHit = true;
            }
        }
    }

    private IEnumerator PushPlayer()
    {
        // prevent the player from moving when being pushed
        PlayerMovement.enabled = false;

        // Push the Player
        PlayerRigidbody.AddForce(Force, ForceMode.Impulse);

        yield return new WaitForSecondsRealtime(PushTimer);

        PlayerMovement.enabled = true;
    }
}
