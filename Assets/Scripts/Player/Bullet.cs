using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 5f;
    public int damage = 10;
    public float slowDuration = 1f;

    void Start()
    {
        // Bullet destory time
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // bullet moving
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Check whether the collision object has a Health component
        health_component targetHealth = other.GetComponent<health_component>();
        if (targetHealth != null)
        {
            if (!targetHealth.isPlayer)
            {
                // Debug.Log("i got here");
                if (targetHealth.getCurrentHealth() - damage <= 0)
                {
                    targetHealth.SetCurrentHealth(1);
                    // Debug.Log("i got here1");
                }
                else
                {
                    targetHealth.ReduceCurrentHealth(damage);
                    // Debug.Log("i got here2");
                }
            }
        }
        Destroy(gameObject);
    }
}