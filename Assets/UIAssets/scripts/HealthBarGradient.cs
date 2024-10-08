using UnityEngine;
using UnityEngine.UI;

public class HealthBarGradient : MonoBehaviour
{
    private Image healthBarImage;
    public Gradient healthGradient;

    void Start()
    {
        healthBarImage = GetComponent<Image>();
        healthBarImage.color = healthGradient.Evaluate(1f); // Start with full health color
    }

    public void SetHealth(float healthPercentage)
    {
        if (float.IsNaN(healthPercentage) || float.IsInfinity(healthPercentage))
        {
            Debug.Log("UI - healthPercentage is not a valid number, probably just initing");
            healthPercentage = 0f; // Set a default value
        }

        healthPercentage = Mathf.Clamp01(healthPercentage);

        // Now safe to use healthPercentage
        healthBarImage.fillAmount = healthPercentage;
        healthBarImage.color = healthGradient.Evaluate(healthPercentage);
    }
}
