using UnityEngine;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI modeText;

    private Rigidbody playerRb;

    void Start()
    {
        if (playerMovement != null)
        {
            playerRb = playerMovement.GetComponent<Rigidbody>();
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
}
