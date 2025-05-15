using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerScript playerScript;

    [Header("Player Stats")]
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI modeText;

    private Rigidbody playerRb;


    [Header("Health UI")]
    public TextMeshProUGUI healthText;
    public Slider healthBar;

    [Header("Stamina UI")]
    public TextMeshProUGUI staminaText;
    public Slider staminaBar;

    void Start()
    {
        if (playerMovement != null)
        {
            playerRb = playerMovement.GetComponent<Rigidbody>();
        }

        if (playerScript != null)
        {
            playerScript.onHealthChanged += UpdateHealthUI;
            playerScript.onStaminaChanged += UpdateStaminaUI;

            // Initialize UI
            UpdateHealthUI(playerScript.currentHealth, playerScript.maxHealth);
            UpdateStaminaUI(playerScript.currentStamina, playerScript.maxStamina);
        }
    }

    void Update()
    {
        if (playerMovement == null || playerRb == null) return;

        // Get horizontal (flat) speed
        Vector3 flatVelocity = new Vector3(playerRb.linearVelocity.x, 0f, playerRb.linearVelocity.z);
        float speed = flatVelocity.magnitude;

        if (speedText != null)
            speedText.text = $"Speed: {speed:F1}";

        if (modeText != null)
            modeText.text = $"{playerMovement.movementMode}";
    }

    void UpdateHealthUI(float current, float max)
    {
        if (healthText != null)
            healthText.text = $"Health: {current:F0} / {max}";

        if (healthBar != null)
            healthBar.value = current / max;
    }

    void UpdateStaminaUI(float current, float max)
    {
        if (staminaText != null)
            staminaText.text = $"Stamina: {current:F0} / {max}";
        
        if (staminaBar != null)
            staminaBar.value = current / max;
    }
}
