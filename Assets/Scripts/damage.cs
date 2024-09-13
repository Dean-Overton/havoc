using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damage : MonoBehaviour
{
    public int bulletDamage = 2;
    // Start is called before the first frame update
    void OnCollisionEnter(Collision collision)
    {
        // Check if the object hit has a HealthComponent
        //Debug.Log($"Applying {bulletDamage} damage beforre searching for component ");
        health_component healthComponent = collision.gameObject.GetComponent<health_component>();

        if (healthComponent != null)
        {
            // If the object has a HealthComponent, apply damage
            healthComponent.ReduceCurrentHealth(bulletDamage);
            Debug.Log($"Applying {bulletDamage} damage to {collision.gameObject.name}");
        }

    }
}