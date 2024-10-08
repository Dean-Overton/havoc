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
    private Coroutine despawnCoroutine;  // To keep track of the despawn coroutine

    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint; // The point where bullets are spawned
    public float bulletSpeed = 20f; // Speed at which the bullet will travel
    public float spawnOffset = 1f; // Distance to spawn the bullet ahead of the player

    [SerializeField] private string bulletSoundName = "LaserShot";
    private GameObject bin;

    void Start()
    {
        // Get the Animator component on the player object
        animator = GetComponent<Animator>();
        bin = GameObject.Find("_BIN_");
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
            else if (currentGun != null && isShootingCoroutineRunning)
            {
                // If the player clicks again, restart the firing process
                StartCoroutine(ShootingCoroutine());
            }

            // Cancel any despawn coroutine if we're firing again
            if (despawnCoroutine != null)
            {
                StopCoroutine(despawnCoroutine);  // Stop any active despawn coroutine
                isDespawningGun = false;  // Reset despawning flag
            }
        }

        // Stop shooting when the player releases right-click
        if (Input.GetMouseButtonUp(1) && isShootingCoroutineRunning)
        {
            // Start the despawn coroutine when firing stops
            if (!isDespawningGun)
            {
                despawnCoroutine = StartCoroutine(StopAndHoldGunCoroutine());
            }
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
            if (Time.time >= nextFireTime)
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

                // Play the shooting sound
                AudioManager.instance.Play(bulletSoundName);
                Debug.Log("Playing sound: " + bulletSoundName);

                nextFireTime = Time.time + fireRate;
            }

            yield return null; // Wait for the next frame
        }

        // Once the player releases the button, hold the gun up for a few seconds before despawning
        if (!isDespawningGun)
        {
            despawnCoroutine = StartCoroutine(StopAndHoldGunCoroutine());
        }
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
