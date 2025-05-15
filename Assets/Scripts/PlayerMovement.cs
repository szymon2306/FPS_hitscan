using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    private PlayerScript playerScript;
    Rigidbody rb;

    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float staminaDrainRate;

    public float groundDrag;


    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    
    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;

    public MovementMode movementMode;
    public enum MovementMode
    {
        Walking,
        Sprinting,
        Crouching,
        Air
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        grounded = true; // start grounded
        readyToJump = true; // start ready to jump

        startYScale = transform.localScale.y; // store the original y scale of the player

        playerScript = GetComponent<PlayerScript>();
    }

    private void Update()
    {
        bool isTryingToSprint = Input.GetKey(sprintKey);
        bool canDrainStamina = isTryingToSprint && playerScript.currentStamina > 0;

        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        if (canDrainStamina && (movementMode == MovementMode.Sprinting || movementMode == MovementMode.Air))
        {
            playerScript.UseStamina(Time.deltaTime * staminaDrainRate); // Adjust rate as needed
        }

        if (playerScript.currentStamina <= 0 && movementMode == MovementMode.Sprinting)
        {
            movementMode = grounded ? MovementMode.Walking : MovementMode.Air;
            moveSpeed = walkSpeed;
        }

        // apply drag on ground
        if (grounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse); // push down to prevent clipping through the ground
        }

        // stop crouching
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }


    }

    public void StateHandler()
    {
        // Mode - Sprinting
        if (grounded && Input.GetKey(sprintKey) && playerScript.currentStamina > 0)
        {
            movementMode = MovementMode.Sprinting;
            moveSpeed = sprintSpeed;
        }
        // Mode - Walking
        else if (grounded)
        {
            movementMode = MovementMode.Walking;
            moveSpeed = walkSpeed;
        }
        // Mode - Air
        else 
        {
            movementMode = MovementMode.Air;
        }

        // Mode - Crouching
        if (Input.GetKey(crouchKey))
        {
            movementMode = MovementMode.Crouching;
            moveSpeed = crouchSpeed;
        }
    }


    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // if moving on slope, change move direction to slope direction
        if (OnSlope() && !exitingSlope)
        {
            moveDirection = GetSlopeMoveDirection() * moveSpeed;

            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.linearVelocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force); // prevent jumping on slope
            }
        }
        

        // move the player
        // on ground
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        // in air
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        // turn gravity off when on a slope
        rb.useGravity = !OnSlope();
        
    }

    private void SpeedControl()
    {
        // limit velocity on a slope
        if (OnSlope() && !exitingSlope)
        {
            if(rb.linearVelocity.magnitude > moveSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
            }
        }

        else
        {
            // limit velocity if needed
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
            // limit y velocity if needed
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

}

