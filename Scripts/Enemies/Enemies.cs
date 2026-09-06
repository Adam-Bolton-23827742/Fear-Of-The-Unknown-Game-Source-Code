using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemies : MonoBehaviour
{
    public EnemyObject Enemy;
    public int CurrentHealth;
    private NavMeshAgent NavMeshAgent;
    private GameObject Player;
    public int CurrentState;
    private Animator Animator;
    private Animator PlayerAnimator;
    [HideInInspector] public bool CanSeePlayer;
    [SerializeField] private LayerMask PlayerLayer;
    private EnemyLOS EnemyLOS;
    [SerializeField] Collider HeadCollider;
    private AudioSource AudioSource, PlayerAudioSource;
    [SerializeField] private AudioClip Idle1, Idle2, Idle3, Attack1, Attack2, Attack3, DeathSound, Hit1, Hit2, Hit3, Walk, PlayerHit;
    [SerializeField] private float SoundDelay;
    private bool IsRunning;
    private PlayerStats PlayerStats;
    private Rigidbody PlayerRigidbody;

    void Start()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        NavMeshAgent.speed = Enemy.MovementSpeed;
        Player = GameObject.Find("Player");
        CurrentHealth = Enemy.MaxHealth;
        Animator = GetComponent<Animator>();
        EnemyLOS = GetComponent<EnemyLOS>();
        AudioSource = GetComponent<AudioSource>();
        PlayerAudioSource = Player.GetComponent<AudioSource>();
        PlayerStats = Player.GetComponent<PlayerStats>();
        PlayerAnimator = Player.GetComponentInChildren<Animator>();
        PlayerRigidbody = Player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentState != 2)
        {
            // Change to attack state when near the player
            if (Vector3.Distance(transform.position, Player.transform.position) <= Enemy.AttackDistance && CanSeePlayer && CurrentState != 4 && PlayerStats.CurrentHealth > 0)
            {
                CurrentState = 3;
            }

            // if the enemy has no health
            if (CurrentHealth <= 0)
            {
                // Kill enemy
                CurrentState = 2;
            }

            // Idle if the player is dead
            if (PlayerStats.CurrentHealth <= 0)
            {
                CurrentState = 0;
            }
        }

        // Enemy state machine
        switch (CurrentState)
        {
            // Idle state
            case 0:
                NavMeshAgent.ResetPath();

                // Set the idle animation
                Animator.SetInteger("State", 1);

                // Play a random idle sound
                if (!IsRunning && PlayerStats.CurrentHealth > 0)
                {
                    StartCoroutine(Sound());
                }

                break;
            // Move state
            case 1:
                NavMeshAgent.destination = Player.transform.position;

                // Set the walk animation
                Animator.SetInteger("State", 2);

                break;
            // Death state
            case 2:
                // Disable navmesh agent so it stops moving away from other enemies
                NavMeshAgent.enabled = false;

                // Set the death animation
                Animator.SetInteger("State", 5);

                // Disable all collisions to prevent softlocking and being pushed around
                GetComponent<CapsuleCollider>().enabled = false;

                if (HeadCollider != null)
                {
                    HeadCollider.enabled = false;
                }

                // Reset layer so they do not interfer with the player aiming at closest target
                gameObject.layer = 0;

                EnemyLOS.enabled = false;
                enabled = false;

                break;
            // Attack state
            case 3:
                NavMeshAgent.ResetPath();

                // player the attack animation
                Animator.SetInteger("State", 3);

                transform.LookAt(new Vector3(Player.transform.position.x, transform.position.y, Player.transform.position.z));
                break;
            // Hit state
            case 4:
                NavMeshAgent.ResetPath();

                // player the hit animation
                Animator.SetInteger("State", 4);

                break;
        }
    }

    public void Attack()
    {
        // Damage the player if in range
        if (Vector3.Distance(transform.position, Player.transform.position) <= Enemy.AttackDistance && CanSeePlayer && PlayerStats.CurrentHealth > 0)
        {
            PlayerRigidbody.isKinematic = true;
            PlayerStats.CurrentHealth -= Enemy.Damage;
            PlayerAudioSource.clip = PlayerHit;
            PlayerAudioSource.Play();

            if (PlayerStats.CurrentHealth > 0)
            {
                // Play the player hit animation
                PlayerAnimator.SetTrigger("Hit");
                PlayerAnimator.SetInteger("State", -1);
                PlayerStats.BeingHit = true;
            }
        }
    }

    public void FollowPlayer()
    {
        // Move towards the player if they are not within the attack radius
        if (Vector3.Distance(transform.position, Player.transform.position) > Enemy.AttackDistance && CurrentState != 2)
        {
            CurrentState = 1;
        }

        // Attack if the player is within the attack radius
        else
        {
            if (PlayerStats.CurrentHealth > 0)
            {
                CurrentState = 3;
            }
        }
    }

    public IEnumerator Sound()
    {
        IsRunning = true;

        // Generate a random number between 0 and 2
        int Sound = Random.Range(0, 3);

        // Set the audio clip based on the random number and current state
        switch (CurrentState)
        {
            // Idle state
            case 0:
                switch (Sound)
                {
                    case 0:
                        AudioSource.clip = Idle1;
                        break;
                    case 1:
                        AudioSource.clip = Idle2;
                        break;
                    case 2:
                        AudioSource.clip = Idle3;
                        break;
                }
                break;
            // Walk state
            case 1:
                AudioSource.clip = Walk;
                break;
            // Death state
            case 2:
                AudioSource.clip = DeathSound;
                break;
            // Attack state
            case 3:
                switch (Sound)
                {
                    case 0:
                        AudioSource.clip = Attack1;
                        break;
                    case 1:
                        AudioSource.clip = Attack2;
                        break;
                    case 2:
                        AudioSource.clip = Attack3;
                        break;
                }
                break;
            // Hit state
            case 4:
                switch (Sound)
                {
                    case 0:
                        AudioSource.clip = Hit1;
                        break;
                    case 1:
                        AudioSource.clip = Hit2;
                        break;
                    case 2:
                        AudioSource.clip = Hit3;
                        break;
                }
                break;
        }

        AudioSource.Play();

        yield return new WaitForSeconds(SoundDelay);

        IsRunning = false;
    }
}
