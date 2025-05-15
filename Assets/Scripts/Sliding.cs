using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    private Rigidbody rb;
    private PlayerMovement playerMovement;

    private PlayerScript playerScript;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;
    
    public float slideYScale;
    private float startYScale;

    public float staminaDrainRate; // Adjust this value to control the stamina drain rate

    [Header("Keybinds")]
    public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;

    private bool sliding;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
        playerScript = GetComponent<PlayerScript>();
        startYScale = player.localScale.y;
    }

    private void Update()
    {
        bool isTryingToSlide = Input.GetKey(slideKey);
        bool canDrainStamina = isTryingToSlide && playerScript.currentStamina => staminaDrainRate;

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (canDrainStamina && Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0))
        {
            playerScript.UseStamina(staminaDrainRate); // Adjust rate as needed
            StartSlide();
        }
        
        /*
        if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0))
        {
            StartSlide();
        }*/

        if (Input.GetKeyUp(slideKey) && sliding)
        {
            StopSlide();
        }
    }

    private void FixedUpdate() 
    {
        if(sliding)
        {
            SlidingMovement();
        }
    }

    private void StartSlide()
    {
        sliding = true;

        player.localScale = new Vector3(player.localScale.x, slideYScale, player.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse); // add a downward force to prevent the player from getting stuck in the ground

        slideTimer = maxSlideTime;
    
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

        slideTimer -= Time.deltaTime; // decrease the slide timer

        if (slideTimer <= 0)
        {
            StopSlide();
        }

    }



    private void StopSlide()
    {
        sliding = false;

        player.localScale = new Vector3(player.localScale.x, startYScale, player.localScale.z);
        
    }


}
