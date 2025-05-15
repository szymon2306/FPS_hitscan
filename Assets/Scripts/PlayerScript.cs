using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 10f;
    public float staminaRegenDelay = 2f;

    private float staminaRegenTimer;

    public delegate void OnHealthChanged(float current, float max);
    public event OnHealthChanged onHealthChanged;

    public delegate void OnStaminaChanged(float current, float max);
    public event OnStaminaChanged onStaminaChanged;

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        staminaRegenTimer = 0f;
    }

    void Update()
    {
        RegenerateStamina();
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void UseStamina(float amount)
    {
        if (currentStamina <= 0) return; // No stamina to use
        {
            currentStamina -= amount;
            currentStamina = Mathf.Max(currentStamina, 0);
            staminaRegenTimer = 0f;
            onStaminaChanged?.Invoke(currentStamina, maxStamina);
        }
    }

    void RegenerateStamina()
    {
        if (currentStamina < maxStamina)
        {
            staminaRegenTimer += Time.deltaTime;

            if (staminaRegenTimer >= staminaRegenDelay)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
                onStaminaChanged?.Invoke(currentStamina, maxStamina);
            }
        }
    }

    void Die()
    {
        Debug.Log("Player died.");
        // Add death logic here (disable movement, play animation, etc.)
    }
}

