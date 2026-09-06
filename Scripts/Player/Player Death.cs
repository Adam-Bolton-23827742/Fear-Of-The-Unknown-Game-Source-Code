using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerDeath : MonoBehaviour
{
    private Rigidbody RB;
    private Animator Animator;
    [SerializeField] private float AlphaSpeed;
    [SerializeField] private CanvasGroup DeathScreenPanel;
    [SerializeField] private GameObject DeathScreenUI, RetryButton;
    private PlayerStats PlayerStats;
    private AudioSource AudioSource;
    [SerializeField] private AudioClip DeathSound;
    private bool IsDead;
    [SerializeField] private GameObject[] Weapons;

    void Start()
    {
        RB = GetComponentInParent<Rigidbody>();
        Animator = GetComponent<Animator>();
        PlayerStats = GetComponentInParent<PlayerStats>();
        AudioSource = GetComponentInChildren<AudioSource>();
    }

    void Update()
    {
        // Kill the player if they have no health
        if (PlayerStats.CurrentHealth <= 0 && !IsDead)
        {
            Death();
        }
    }

    private void Death()
    {
        IsDead = true;

        // Disable all player mechanics when dead
        RB.isKinematic = true;
        GetComponentInParent<PlayerMovement>().enabled = false;
        GetComponentInParent<ToggleInventoryMenu>().enabled = false;
        GetComponentInParent<TogglePause>().enabled = false;
        foreach (GameObject Weapon in Weapons)
        {
            Weapon.GetComponent<Shooting>().enabled = false;
        }

        // Play the death animation
        Animator.SetInteger("State", 7);

        // Play the death sound
        AudioSource.clip = DeathSound;
        AudioSource.Play();
    }

    private IEnumerator DeathScreen()
    {
        DeathScreenPanel.gameObject.SetActive(true);

        // Fade in the death screen
        while (DeathScreenPanel.alpha < 1)
        {
            DeathScreenPanel.alpha += AlphaSpeed;
            yield return new WaitForEndOfFrame();
        }

        // Pause the game
        Time.timeScale = 0.0f;

        // Select retry button on death screen
        EventSystem.current.SetSelectedGameObject(RetryButton);

        // Set the death screen UI active
        DeathScreenUI.SetActive(true);
    }
}
