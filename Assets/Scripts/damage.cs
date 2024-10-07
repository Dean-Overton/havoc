using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damage : MonoBehaviour
{
    public int bulletDamage = 2;
    private float bulletSpeed = 20f;
    public int bulletLifetime = 2;

    void Start()
    {
        // Automatically destroy the bullet after 'bulletLifetime' seconds
        Destroy(gameObject, bulletLifetime);
    }

    void Update()
    {
        // Move the bullet forward
        transform.position += transform.forward * bulletSpeed * Time.deltaTime;
    }

    // Use OnTriggerEnter instead of OnCollisionEnter
    void OnTriggerEnter(Collider other)
    {
        // Check if the object hit has a health_component
        health_component healthComponent = other.gameObject.GetComponent<health_component>();

        // Only process objects with a health_component
        if (healthComponent != null)
        {
            // Ensure the health component belongs to a non-player entity
            if (!healthComponent.isPlayer)
            {
                if (healthComponent.getCurrentHealth() - bulletDamage <= 0)
                {
                    healthComponent.SetCurrentHealth(1);
                }
                else
                {
                    healthComponent.ReduceCurrentHealth(bulletDamage);
                }
            }

            // Destroy the bullet after processing damage
            Destroy(gameObject);
        }
    }
}
