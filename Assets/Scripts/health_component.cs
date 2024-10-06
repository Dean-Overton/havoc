using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class health_component : MonoBehaviour
{
    [SerializeField] bool isPlayer = false;
    [SerializeField] int currentHealth = 100;
    [SerializeField] int maxHealth = 100;
    [SerializeField] bool passiveRegen = false;
    [SerializeField] int passiveRegenHealthPerSecond = 1;

    private float regenTimer = 0f; // Timer to track when to regenerate health
    [SerializeField] float regenInterval = 1f; // Time interval for health regeneration (in seconds)

    // the logic to handle when the player is killed to broadcast it to the game controller
    // public delegate void CharacterDied(int characterID);
    // public event CharacterDied OnCharacterDeath;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        // Ensure current health does not exceed the new max health
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
    public void SetCurrentHealth(int amount)
    {
        currentHealth = amount;
        // Ensure current health does not exceed the new max health
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
    public void AddCurrentHealth(int increment)
    {
        currentHealth = Mathf.Clamp(currentHealth + increment, 0, maxHealth); // Ensure health is within valid range
    }

    public void ReduceCurrentHealth(int increment)
    {
        currentHealth = Mathf.Max(currentHealth - increment, 0);
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

        // Destroy the object if health is 0 or below
        if (currentHealth <= 0) {
            if (isPlayer) {
                EventManager.BroadcastCharacterDeath(1);        // Broadcast the player death event
            } else { // Update for when we have multiple enemies
                EventManager.BroadcastCharacterDeath(2);        // Broadcast the enemy death event
            }
            Destroy(gameObject);
        }
    }
}
