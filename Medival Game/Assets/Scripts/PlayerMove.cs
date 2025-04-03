using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 6.0f;
    private float rotationSpeed = 10.0f; // Rotation speed for smooth turning
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    private float currentSpeed; // Variable to handle smooth acceleration/deceleration
    private float acceleration = 5.0f; // Speed to accelerate to max speed
    private float deceleration = 8.0f; // Speed to decelerate to zero

    public Transform cameraTransform; // Reference to the camera's transform

    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    private Animator animator; // Reference to the Animator component

    private bool isJumping = false;  // Flag to check if we are jumping
    bool isMoving = false;  

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        animator = gameObject.GetComponentInChildren<Animator>(); // Get the Animator from the child
        currentSpeed = 0f; // Start with zero speed
    }

    IEnumerator attackDelay(int sec)
    {
        yield return new WaitForSeconds(sec);
        
    }

    void Update()
    {
        // Ground check
        groundedPlayer = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // If grounded, reset vertical velocity to prevent floating after landing
        if (groundedPlayer)
        {
            // Only apply the downward force if the player is grounded to keep them on the ground
            if (playerVelocity.y < 0)
            {
                playerVelocity.y = -2f; // Apply small downward force to ensure player is touching the ground
            }

            // If the player is grounded and was jumping, the animator will transition to the land animation automatically
            if (isJumping)
            {
                isJumping = false;
                // The transition to "Land" will happen in the animator itself, based on the grounded state
            }
        }

        // Jump logic: Trigger jump animation only once
        if (Input.GetButtonDown("Jump") && groundedPlayer && !isJumping)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue); // Apply jump force
            animator.SetTrigger("Jump");  // Trigger jump animation by setting a bool in the animator
            isJumping = true;  // Set flag to indicate we are jumping
        }

        //Attack
        if(Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
            
        }

        // Sprint Logic
        if(Input.GetKeyDown(KeyCode.LeftShift)&&isMoving)
        {
            playerSpeed = 10.0f;
            animator.SetBool("Running", true);
        }

        if(Input.GetKeyUp(KeyCode.LeftShift)||!isMoving)
        {
            playerSpeed = 6.0f;
            animator.SetBool("Running", false);
        }


        // Apply gravity (continuous downward force)
        if (!groundedPlayer) // Only apply gravity if the player is in the air
        {
            playerVelocity.y += gravityValue * Time.deltaTime;
        }

        // Get movement input (WASD)
        float moveDirectionX = Input.GetAxis("Horizontal"); // A/D for left/right
        float moveDirectionZ = Input.GetAxis("Vertical"); // W/S for forward/backward

        // Get the camera's forward and right vectors (relative to the camera's rotation)
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Ensure we move only on the XZ plane
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        // Normalize the vectors
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate movement direction based on camera orientation
        Vector3 move = (cameraForward * moveDirectionZ + cameraRight * moveDirectionX).normalized;

        // Smoothly accelerate and decelerate the player
        if (move.magnitude > 0) // Moving forward
        {
            currentSpeed = Mathf.Lerp(currentSpeed, playerSpeed, acceleration * Time.deltaTime);
            animator.SetBool("Walking", true);
            isMoving = true;
        }
        else // Not moving
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, deceleration * Time.deltaTime);
            animator.SetBool("Walking", false);
            animator.SetBool("Running", false);
            isMoving = false;
        }

        // Apply movement on XZ plane
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Apply vertical movement (jump/fall)
        controller.Move(playerVelocity * Time.deltaTime);

        // Rotate the character to face the direction of movement
        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move); // Rotate in the movement direction
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
