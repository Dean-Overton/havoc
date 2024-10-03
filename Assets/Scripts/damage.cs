using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damage : MonoBehaviour
{
    public int bulletDamage = 2;
    private float bulletSpeed = 20f;

    void Update()
    {
        // Move the bullet forward
        transform.position += transform.forward * bulletSpeed * Time.deltaTime;

        if (transform.position.x > 50 || transform.position.z > 50) {
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        // Check if the object hit has a HealthComponent
        //Debug.Log($"Applying {bulletDamage} damage beforre searching for component ");
        health_component healthComponent = collision.gameObject.GetComponent<health_component>();

        if (healthComponent != null)
        {
            // If the object has a HealthComponent, apply damage
            healthComponent.ReduceCurrentHealth(bulletDamage);
        }

        // Destroy the bullet
        Destroy(gameObject);

    }
}