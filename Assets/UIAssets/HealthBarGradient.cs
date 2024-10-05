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
        healthBarImage.fillAmount = healthPercentage;
        healthBarImage.color = healthGradient.Evaluate(healthPercentage);
    }
}
