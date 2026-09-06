using System.Collections;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    [SerializeField] private float ExplosionRadius;
    [SerializeField] private LayerMask DamageLayer;
    [SerializeField] private int Damage;
    [HideInInspector] public bool Explode;
    private Collider[] BossArray;
    [SerializeField] private Animator PlayerAnimator;
    [SerializeField] private ParticleSystem ExplosionParticle;
    private Transform ExplosionSpawn;

    private void Start()
    {
        ExplosionSpawn = transform.Find("Explosion Spawn");
    }

    void Update()
    {
        BossArray = Physics.OverlapSphere(transform.position, ExplosionRadius, DamageLayer);

        if (Explode)
        {
            ParticleSystem NewExplosion = Instantiate(ExplosionParticle, ExplosionSpawn.position, Quaternion.identity);
            NewExplosion.Play();
            NewExplosion.GetComponent<AudioSource>().Play();

            foreach (Collider C in BossArray)
            {
                if (C != null && C.gameObject != gameObject)
                {
                    if (C.tag == "Boss")
                    {
                        Enemies Enemies = C.gameObject.GetComponent<Enemies>();

                        if (Enemies != null)
                        {
                            Enemies.CurrentHealth -= Damage;
                            Enemies.CurrentState = 4;
                        }
                    }

                    else
                    {
                        PlayerStats Stats = C.gameObject.GetComponentInParent<PlayerStats>();

                        if (Stats != null)
                        {
                            Stats.CurrentHealth -= Damage;

                            // Play the player hit animation
                            PlayerAnimator.SetTrigger("Hit");
                            PlayerAnimator.SetInteger("State", -1);
                            Stats.BeingHit = true;
                        }
                    }
                }
            }

            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
    }
}
