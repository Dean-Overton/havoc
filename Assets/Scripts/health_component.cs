using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class health_component : MonoBehaviour
{
    [SerializeField] int currentHealth = 100;
    [SerializeField] int maxHealth = 100;
    [SerializeField] bool passiveRegen = true;
    [SerializeField] int passiveRegenHealthPerSecond = 1;

    private float regenTimer = 0f; // Timer to track when to regenerate health
    [SerializeField] float regenInterval = 1f; // Time interval for health regeneration (in seconds)

    // the logic to handle when the player is killed to broadcast it to the game controller
    public delegate void PlayerDied();
    public event PlayerDied OnPlayerDeath;

    public void SetCurrentHealth(int increment)
    {
        currentHealth = Mathf.Clamp(increment, 0, maxHealth); // Ensure health is within valid range
    }

    public void SetMaxHealth(int increment)
    {
        maxHealth = increment;
        // Ensure current health does not exceed the new max health
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void AddMaxHealth(int increment)
    {
        maxHealth += increment;
        // Ensure current health does not exceed the new max health
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void AddCurrentHealth(int increment)
    {
        currentHealth = Mathf.Clamp(currentHealth + increment, 0, maxHealth); // Ensure health is within valid range
    }

    public void ReduceMaxHealth(int increment)
    {
        maxHealth = Mathf.Max(maxHealth - increment, 0);
        // Ensure current health does not exceed the new max health
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void ReduceCurrentHealth(int increment)
    {
        currentHealth = Mathf.Max(currentHealth - increment, 0);
        if (currentHealth <= 0)
        {
            OnPlayerDeath.Invoke();     // Sending a public event that the player has died
            Destroy(gameObject);        // Destroying the Player Body Object
        }
    }

    public void regenerateHealth()
    {
        // Add regeneration logic here if needed
    }

    public int getCurrentHealth()
    {
        return currentHealth;
    }

    public int getMaxHealth()
    {
        return maxHealth;
    }

    void Update()
    {
        // Handle passive health regeneration
        if (passiveRegen)
        {
            regenTimer += Time.deltaTime; // Increment the timer

            if (regenTimer >= regenInterval)
            {
                // Regenerate health
                AddCurrentHealth(passiveRegenHealthPerSecond);
                regenTimer = 0f; // Reset the timer
            }
        }

        // Clamp current health to max health
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // Destroy the object if health is 0 or below
        if (currentHealth <= 0)
        {
            OnPlayerDeath.Invoke();     // Sending a public event that the player has died
            Destroy(gameObject);        // Destroying the Player Body Object
        }
    }
}
