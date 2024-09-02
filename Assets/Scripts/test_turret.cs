using UnityEngine;

public class BulletFiringScript : MonoBehaviour
{
    public GameObject bulletPrefab;    // The bullet prefab to instantiate
    public Transform target;           // The target object to aim at
    public float bulletSpeed = 20f;    // The speed at which the bullet will travel
    public float fireRate = 1f;        // How frequently bullets are fired
    public float spawnHeight = 2f;     // Height above the firing object to spawn the bullet
    public int bulletDamage = 50;
    private float nextFireTime = 0f;

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            FireBullet();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void FireBullet()
    {
        // Calculate the position above the firing object
        Vector3 firePosition = transform.position + Vector3.up * spawnHeight;

        // Calculate the direction from the fire position to the target
        Vector3 direction = (target.position - firePosition).normalized;

        // Instantiate the bullet at the fire position
        GameObject bullet = Instantiate(bulletPrefab, firePosition, Quaternion.LookRotation(direction));

        // Set the bullet's velocity to move towards the target
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * bulletSpeed;
        }
        damage dm = bullet.GetComponent<damage>();
        if (dm != null)
        {
            dm.bulletDamage = bulletDamage;
        }
    }
}
