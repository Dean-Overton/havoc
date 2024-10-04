using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIDisplay : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int ammoCapacity;
    public int currentAmmo;
    public UIDataHandler UIDataHandler;
    [Header("UI References")]
    public Image bulletIconPrefab;

    private List<Image> bulletIcons = new List<Image>();

    void Start()
    {
        InitializeAmmoDisplay();
        currentAmmo = UIDataHandler.ammo;
        //ammoCapacity = UIDataHandler. unneeded - scope creep
    }

    void Update()
    {
        UpdateAmmoDisplay();
    }

    void InitializeAmmoDisplay()
    {
        // Clear existing icons
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        bulletIcons.Clear();

        // Instantiate bullet icons
        for (int i = 0; i < ammoCapacity; i++)
        {
            Image bulletIcon = Instantiate(bulletIconPrefab, transform);
            bulletIcon.transform.localScale = Vector3.one;
            bulletIcon.rectTransform.sizeDelta = new Vector2(9, 15); // Match Cell Size
            bulletIcons.Add(bulletIcon);
        }
    }

    void UpdateAmmoDisplay()
    {
        currentAmmo = UIDataHandler.ammo;
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
}
