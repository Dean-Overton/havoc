using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIDisplay : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int ammoCapacity = 20;
    public int currentAmmo;
    public UIDataHandler UIDataHandler;

    [Header("UI References")]
    public Transform ammoDisplayParent;    // Parent for ammo icons
    public Image bulletIconPrefab;
    private List<Image> bulletIcons = new List<Image>();

    [Header("Health Bar Settings")]
    public Image healthBarFillImage;       // Reference to the health bar fill image
    public int currentHealth;
    public int maxHealth;


    void Start()
    {
        currentAmmo = UIDataHandler.ammo;
        currentHealth = UIDataHandler.health;
        maxHealth = UIDataHandler.maxhealth;
        Debug.Log("Start with" + ammoCapacity);
        //ammoCapacity = UIDataHandler.maxAmmo;
        InitializeAmmoDisplay();
        UpdateHealthBar();
    }

    void Update()
    {
        UpdateAmmoDisplay();
        UpdateHealthBar();
    }

    void InitializeAmmoDisplay()
    {
        Debug.Log("init with" +  ammoCapacity);
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
            Debug.Log("drawing");
        }
    }

    void UpdateAmmoDisplay()
    {
        currentAmmo = UIDataHandler.ammo;
        ammoCapacity = UIDataHandler.maxAmmo;
        
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
    }

    void UpdateHealthBar()
    {
        currentHealth = UIDataHandler.health;
        maxHealth = UIDataHandler.maxhealth;

        float healthPercentage = (float)currentHealth / maxHealth;

        // Update the fill amount of the health bar image
        HealthBarGradient gradientControlHP = healthBarFillImage.GetComponent<HealthBarGradient>();
        gradientControlHP.SetHealth(healthPercentage);

    }
}
