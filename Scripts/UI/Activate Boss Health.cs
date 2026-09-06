using UnityEngine;

public class ActivateBossHealth : MonoBehaviour
{
    [SerializeField] private GameObject BossHealthBar;
    [SerializeField] private Doors BossDoor;
    private bool LockDoor;
    private Animator Animator;

    private void Start()
    {
        Animator = BossDoor.gameObject.GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (LockDoor)
        {
            if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Door Close") || Animator.GetCurrentAnimatorStateInfo(0).IsName("Door Close 2"))
            {
                BossDoor.IsLocked = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            BossHealthBar.SetActive(true);
            LockDoor = true;
        }
    }
}
