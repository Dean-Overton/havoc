using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealthBarGradient : MonoBehaviour
{
    private Image healthBarImage;
    public Gradient healthGradient;
    private CanvasGroup healthBarCanvasGroup; // Changed to private since we add this dynamically
    private Coroutine hideHealthBarCoroutine;

    void Awake()
    {
        healthBarImage = GetComponent<Image>();

        if (healthBarImage == null)
        {
            Debug.LogError("Image component not found on this GameObject.");
            return;
        }

        if (healthGradient == null)
        {
            Debug.LogError("Health gradient is not assigned.");
            return;
        }

        // Try to get the CanvasGroup from the parent
        healthBarCanvasGroup = GetComponentInParent<CanvasGroup>();

        // If it doesn't exist, add a CanvasGroup component to the parent
        if (healthBarCanvasGroup == null)
        {
            healthBarCanvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Start with the health bar hidden
        healthBarCanvasGroup.alpha = 0f;
        healthBarImage.color = healthGradient.Evaluate(1f); // Start with full health color
    }
    private void OnDisable() {
        // Stop the coroutine when the object is disabled
        if (hideHealthBarCoroutine != null)
        {
            StopCoroutine(hideHealthBarCoroutine);
        }
    }
    public void SetHealth(float healthPercentage)
    {
        healthPercentage = Mathf.Clamp01(healthPercentage);
        healthBarImage.fillAmount = healthPercentage;
        healthBarImage.color = healthGradient.Evaluate(healthPercentage);

        // Show the health bar when taking damage
        ShowHealthBar();

        // If there's an active coroutine to hide the bar, stop it so it doesn't hide too soon
        if (hideHealthBarCoroutine != null)
        {
            StopCoroutine(hideHealthBarCoroutine);
        }

        // Start a coroutine to hide the health bar after a delay
        hideHealthBarCoroutine = StartCoroutine(HideHealthBarAfterDelay(2f)); // Hide after 2 seconds of inactivity
    }

    private void ShowHealthBar()
    {
        healthBarCanvasGroup.alpha = 1f; // Make the health bar visible
    }

    private IEnumerator HideHealthBarAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        healthBarCanvasGroup.alpha = 0f; // Hide the health bar after delay
    }
}
