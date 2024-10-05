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

    private int characterID;

    private float regenTimer = 0f; // Timer to track when to regenerate health
    [SerializeField] float regenInterval = 1f; // Time interval for health regeneration (in seconds)

    void Awake() {
        if (isPlayer) {
            characterID = 1;
        } else {
            characterID = 2;
        }
        currentHealth = maxHealth;
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
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
            DIE();
        }

        if (gameObject.GetComponent<Transform>().position.y < -10) {
            DIE();
        }
    }

    public void DIE() {
        EventManager.BroadcastCharacterDeath(characterID);        // Broadcast the character death event
        Destroy(gameObject);
    }
}
