using UnityEngine;

public class Footstep : MonoBehaviour
{
    [SerializeField] private AudioClip AudioClip;
    [SerializeField] private AudioSource AudioSource;
    private Animator Animator;
    private Rigidbody Rigidbody;
    private PlayerStats Stats;

    private void Start()
    {
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponentInParent<Rigidbody>();
        Stats = GetComponentInParent<PlayerStats>();
    }

    private void FootstepSound()
    {
        AudioSource.clip = AudioClip;
        AudioSource.Play();
    }

    // Called at the end of the hit animation to prevent being stuck in that animation and to unfreeze the player
    private void AfterDamage()
    {
        Stats.BeingHit = false;
        Rigidbody.isKinematic = false;
    }
}
