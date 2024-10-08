using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUIDataHandler : MonoBehaviour
{
    public int health;
    public int maxHealth;
    private health_component healthComp;
    private EnemyHealthBarGradient enemyHealthBar;

    // Start is called before the first frame update
    void Start()
    {
        healthComp = GetComponent<health_component>();
        enemyHealthBar = GetComponentInChildren<EnemyHealthBarGradient>(); // Assign to class-level variable
    }

    // Update is called once per frame
    void Update()
    {
        health = healthComp.getCurrentHealth();
        maxHealth = healthComp.getMaxHealth();
        float healthPercentage = (float)health / maxHealth;

        if (enemyHealthBar != null)
        {
            enemyHealthBar.SetHealth(healthPercentage);
        }
    }
}
