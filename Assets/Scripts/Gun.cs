using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint; // The point where bullets are spawned
    public float fireRate = 0.1f;
    public float bulletSpeed = 20f; // Speed at which the bullet will travel
    public float spawnOffset = 1f; // Distance to spawn the bullet ahead of the player
    private float nextFireTime;
    private GameObject bin;

    void Start()
    {
        bin = GameObject.Find("_BIN_");
    }
    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime) // Check if fire button is pressed and time allows firing
        {
            Shoot();
            nextFireTime = Time.time + fireRate; // Update the next firing time
        }
    }

    void Shoot()
    {
        // Calculate the spawn position ahead of the player
        Vector3 spawnPosition = bulletSpawnPoint.position + bulletSpawnPoint.forward * spawnOffset;
        Vector3 spawnDirection = -bulletSpawnPoint.forward;
        // Create the rotation that looks in the spawn direction
        Quaternion spawnRotation = Quaternion.LookRotation(-spawnDirection, Vector3.up);
        
        // Instantiate the bullet at the calculated spawn position with the correct rotation
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, spawnRotation, bin.transform);
        
        // Optionally, set the speed of the bullet if needed
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.speed = bulletSpeed;
        }

        // Optionally, if you have additional bullet setup like damage, you can handle it here
        // damage dm = bullet.GetComponent<damage>();
        // if (dm != null)
        // {
        //     dm.bulletDamage = bulletDamage; // Ensure bulletDamage is defined in your script
        // }
    }
}
