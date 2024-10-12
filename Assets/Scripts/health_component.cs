using System;
using System.Collections;
using UnityEngine;

public class health_component : MonoBehaviour
{
    [SerializeField] public bool isPlayer = false;
    [SerializeField] int currentHealth = 100;
    [SerializeField] int maxHealth = 100;
    [SerializeField] bool passiveRegen = false;
    [SerializeField] int passiveRegenHealthPerSecond = 1;
    [SerializeField] public GameObject deathEffectPrefab;  // Reference to the particle system prefab
    [SerializeField] private int characterID = 2;

    [SerializeField] public float destroyDelay = 0.2f;

    private float regenTimer = 0f; // Timer to track when to regenerate health
    [SerializeField] float regenInterval = 1f; // Time interval for health regeneration (in seconds)

    [SerializeField] string reduceDamageSoundName = "MeatPunchSound";
    [SerializeField] string deathSoundName = "DeathSound";

    void Awake() {
        if (isPlayer) {
            characterID = 1;
        }
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
    public void ReduceCurrentHealth(int increment, Vector3? dashDirection = null)
    {
        currentHealth = Mathf.Max(currentHealth - increment, 0);

        // Play the reduce damage sound
        AudioManager.instance.Play(reduceDamageSoundName);

        // If health is zero, DIE and pass the dash direction
        if (currentHealth <= 0)
        {
            DIE(dashDirection);
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

        // Destroy the object if health is 0 or below
        if (currentHealth <= 0) {
            DIE();
        }

        if (gameObject.GetComponent<Transform>().position.y < -10) {
            DIE();
        }
    }
    public event Action onDeath;
    private bool isDead = false;
    public void DIE(Vector3? dashDirection = null)
    {
        if (isDead) return;
        isDead = true;

        // Play the death sound
        AudioManager.instance.Play(deathSoundName);

        if (deathEffectPrefab != null)
        {
            GameObject deathEffect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

            // Apply the dash direction to the particle effect
            if (dashDirection.HasValue)
            {
                var ps = deathEffect.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    var velocityOverLifetime = ps.velocityOverLifetime;
                    velocityOverLifetime.enabled = true;
                    velocityOverLifetime.x = dashDirection.Value.x * 20f; // Adjust the multiplier as needed
                    velocityOverLifetime.y = dashDirection.Value.y * 20f;
                    velocityOverLifetime.z = dashDirection.Value.z * 20f + 3f;
                }
            }
        }

        EventManager.BroadcastCharacterDeath(characterID); // Broadcast the character death event
        onDeath?.Invoke(); // Invoke the onDeath event

        StartCoroutine(DestroyAfterDelay(destroyDelay));
    }


    private IEnumerator DestroyAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
