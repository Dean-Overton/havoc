using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour
{
    public GameObject gunPrefab;  // Assign your gun prefab in the inspector
    public Transform handBone;    // Drag the character's hand bone here in the inspector
    public float fireRate = 0.1f; // How fast the gun shoots (in seconds)
    public float holdGunDuration = 1f;  // Time to hold the gun after shooting stops
    private GameObject currentGun;
    private bool isSpawningGun = false;  // Flag to prevent multiple gun spawns
    public bool isShootingCoroutineRunning = false;  // Flag to prevent multiple shooting coroutines
    private bool isDespawningGun = false;  // Flag to prevent multiple despawns
    private float nextFireTime = 0f;
    private Animator animator;
    public bool isShooting;
    public Vector3 gunPosOffset = new Vector3(0.0058f, -0.0239f, 0.0169f); // Normal gun offset to get it in her hands
    public Quaternion gunPosRotation = Quaternion.Euler(0f, 108.9f, 90f);
    public Vector3 gunScale = new Vector3(1.2f, 1.2f, 1.2f);

    void Start()
    {
        // Get the Animator component on the player object
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Right-click to both spawn the gun and start firing
        if (Input.GetMouseButtonDown(1)) // Right-click (Mouse button 1)
        {
            if (currentGun == null && !isSpawningGun)
            {
                StartCoroutine(SpawnAndFireGunCoroutine());
            }
            else if (currentGun != null && !isShootingCoroutineRunning)
            {
                StartCoroutine(ShootingCoroutine());
            }
        }

        // Stop shooting when the player releases right-click
        if (Input.GetMouseButtonUp(1) && isShootingCoroutineRunning)
        {
            StartCoroutine(StopAndHoldGunCoroutine());
        }
    }

    private IEnumerator SpawnAndFireGunCoroutine()
    {
        isSpawningGun = true;

        // Spawn the gun in the character's hand
        currentGun = Instantiate(gunPrefab, handBone.position, handBone.rotation);
        currentGun.transform.SetParent(handBone);  // Attach to the hand bone
        ChangeGunTransform(currentGun);

        yield return new WaitForEndOfFrame(); // Ensure the gun is fully instantiated

        isSpawningGun = false;

        // Immediately start firing after the gun is spawned
        if (!isShootingCoroutineRunning)
        {
            StartCoroutine(ShootingCoroutine());
        }
    }

    private IEnumerator ShootingCoroutine()
    {
        isShootingCoroutineRunning = true;

        // Start shooting animation
        isShooting = true;
        animator.SetBool("isShooting", isShooting);


        GetComponent<CharacterFacing>().RotateTowardMouse = true;

        // Keep firing as long as right-click is held down
        while (Input.GetMouseButton(1))
        {
            if (Time.time > nextFireTime)
            {
                nextFireTime = Time.time + fireRate;
                FireGun();
            }

            yield return null; // Wait for the next frame
        }

        // Once the player releases the button, hold the gun up for a few seconds before despawning
        StartCoroutine(StopAndHoldGunCoroutine());
    }

    private IEnumerator StopAndHoldGunCoroutine()
    {
        if (isDespawningGun) yield break;  // Prevent multiple despawn routines
        isDespawningGun = true;

        // Continue holding the gun up for a couple more seconds
        yield return new WaitForSeconds(holdGunDuration);

        // Now stop shooting animation and despawn the gun
        StopShooting();
        DespawnGun();

        isDespawningGun = false;
    }

    void FireGun()
    {
        // This method handles the firing logic, e.g., spawning bullets from the gun
        Debug.Log("Firing the gun!");

        // Add bullet instantiation logic here if necessary
        // Example:
        /*
        GameObject bullet = Instantiate(bulletPrefab, currentGun.transform.Find("Muzzle").position, currentGun.transform.Find("Muzzle").rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.velocity = currentGun.transform.forward * bulletSpeed;
        */
    }

    void StopShooting()
    {
        // Stop shooting animation and reset shooting state
        isShooting = false;
        animator.SetBool("isShooting", isShooting);

        GetComponent<CharacterFacing>().RotateTowardMouse = false;

        isShootingCoroutineRunning = false; // Reset coroutine flag
    }

    void DespawnGun()
    {
        // Destroy the gun object after a few seconds
        if (currentGun != null)
        {
            Destroy(currentGun);
            currentGun = null;
        }
    }

    void ChangeGunTransform(GameObject spawnedGun)
    {
        // Change the position (relative to the parent, i.e., the hand)
        spawnedGun.transform.localPosition += gunPosOffset;

        // Change the rotation (relative to the parent)
        spawnedGun.transform.localRotation = gunPosRotation;

        // Change the scale of the gun
        spawnedGun.transform.localScale = gunScale;
    }
}
