using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIDisplay : MonoBehaviour
{
    public UIDataHandler UIDataHandler;
    [Header("Ammo Settings")]
    public int ammoCapacity = 20;
    public int currentAmmo;


    [Header("Ammo UI References")]
    public Transform ammoDisplayParent;    // Parent for ammo icons
    public Image bulletIconPrefab;
    private List<Image> bulletIcons = new List<Image>();
    public Image EmptyIcon;

    [Header("Health Bar Settings")]
    public Image healthBarFillImage;       // Reference to the health bar fill image
    public int currentHealth;
    public int maxHealth;

    [Header("Dash Settings")]
    public int maxDashes = 3;
    public int currentDashes;

    [Header("Dash UI References")]
    public Transform dashIconsParent;    // Parent for dash icons
    public Image dashIconPrefab;
    private List<Image> dashIcons = new List<Image>();

    void Start()
    {
        if (UIDataHandler == null)
        {
            Debug.LogError("UI - UIDataHandler is not assigned in UIDisplay.");
            return;
        }

        // Initialize ammo
        currentAmmo = UIDataHandler.ammo;
        ammoCapacity = UIDataHandler.maxAmmo;
        InitializeAmmoDisplay();

        // Initialize health
        currentHealth = UIDataHandler.health;
        maxHealth = UIDataHandler.maxhealth;
        UpdateHealthBar();

        // Initialize dashes
        currentDashes = UIDataHandler.dashes;   
        maxDashes = UIDataHandler.maxDashes;    
        InitializeDashDisplay();
    }

    void Update()
    {
        UpdateAmmoDisplay();
        UpdateHealthBar();
        UpdateDashDisplay();
    }

    void InitializeAmmoDisplay()
    {
        Debug.Log("UI - init ammo with " + ammoCapacity);
        // Clear existing icons
        foreach (Transform child in ammoDisplayParent)
        {
            Destroy(child.gameObject);
        }
        bulletIcons.Clear();

        // Instantiate bullet icons under the ammoDisplayParent
        for (int i = 0; i < ammoCapacity; i++)
        {
            Image bulletIcon = Instantiate(bulletIconPrefab, ammoDisplayParent);
            bulletIcon.transform.localScale = Vector3.one;
            bulletIcon.rectTransform.sizeDelta = new Vector2(9, 15);
            bulletIcons.Add(bulletIcon);
        }
    }

    void UpdateAmmoDisplay()
    {
        currentAmmo = UIDataHandler.ammo;
        ammoCapacity = UIDataHandler.maxAmmo;
        Color EmptyImageColor = EmptyIcon.color;

        if (bulletIcons.Count != ammoCapacity)
        {
            InitializeAmmoDisplay();
        }

        for (int i = 0; i < bulletIcons.Count; i++)
        {

            if (i < currentAmmo)
            {
                // Set bullet icon to fully visible red
                bulletIcons[i].color = new Color(1f, 0f, 0f, 1f);
            }
            else
            {
                // Set bullet icon to transparent red
                bulletIcons[i].color = new Color(1f, 0f, 0f, 0.4f);
            }
        }
        Color emptyImageColor = EmptyIcon.color;
        emptyImageColor.a = (currentAmmo == 0) ? 1f : 0f; // Alpha should be between 0 and 1
        EmptyIcon.color = emptyImageColor; // Apply the updated color back to the EmptyIcon

    }

    void InitializeDashDisplay()
    {
        Debug.Log("UI - init dashes with " + maxDashes);
        // Clear existing icons
        foreach (Transform child in dashIconsParent)
        {
            Destroy(child.gameObject);
        }
        dashIcons.Clear();

        // Instantiate dash icons under the dashIconsParent
        for (int i = 0; i < maxDashes; i++)
        {
            Image dashIcon = Instantiate(dashIconPrefab, dashIconsParent);
            dashIcon.transform.localScale = Vector3.one;
            // Adjust size if necessary
            // dashIcon.rectTransform.sizeDelta = new Vector2(width, height); // Set appropriate size
            dashIcons.Add(dashIcon);
        }
    }

    void UpdateDashDisplay()
    {
        currentDashes = UIDataHandler.dashes;   
        maxDashes = UIDataHandler.maxDashes;    

        if (dashIcons.Count != maxDashes)
        {
            InitializeDashDisplay();
        }

        for (int i = 0; i < dashIcons.Count; i++)
        {
            if (i < currentDashes)
            {
                // Set dash icon to fully visible
                dashIcons[i].color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                // Set dash icon to dimmed or transparent
                dashIcons[i].color = new Color(1f, 1f, 1f, 0.4f);
            }
        }
    }

    void UpdateHealthBar()
    {
        currentHealth = UIDataHandler.health;
        maxHealth = UIDataHandler.maxhealth;

        float healthPercentage = (float)currentHealth / maxHealth;

        // Update the fill amount of the health bar image
        HealthBarGradient gradientControlHP = healthBarFillImage.GetComponent<HealthBarGradient>();
        if (gradientControlHP != null)
        {
            gradientControlHP.SetHealth(healthPercentage);
        }
        else
        {
            Debug.LogError("HealthBarGradient component not found on healthBarFillImage.");
        }
    }
}
