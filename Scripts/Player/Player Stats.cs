using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int CurrentHealth, MaxHealth;
    [HideInInspector] public bool BeingHit;

    // Update is called once per frame
    void Update()
    {
        // Prvent the player's current health exceeding the max health
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
    }
}
