using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; // Include for UI handling

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float WalkSpeed = 5f;
    public float SprintMultiplier = 2f;
    public float JumpForce = 5f;
    public float GroundCheckDistance = 1.5f;
    public float LookSensitivityx = 1f;
    public float LookSensitivityy = 1f;
    public float MinYLookAngle = -90f;
    public float MaxYLookAngle = 90f;
    public Transform PlayerCamera;
    public float Gravity = -9.8f;
    public Vector3 StartingPosition; // Store the starting position
    public Text WinText; // UI Text for winning message

    private Vector3 velocity;
    private float verticalRotation = 0f;
    private CharacterController characterController;

    // New Input System fields
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction lookAction;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        // Set up input actions
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];
        lookAction = playerInput.actions["Look"];

        StartingPosition = transform.position; // Store the starting position

        Cursor.lockState = CursorLockMode.Locked;
        WinText.gameObject.SetActive(false); // Hide win text initially
    }

    void Update()
    {
        // Get movement inputs
        Vector2 movementInput = moveAction.ReadValue<Vector2>();
        float horizontalMovement = movementInput.x;
        float verticalMovement = movementInput.y;

        Vector3 moveDirection = transform.forward * verticalMovement + transform.right * horizontalMovement;
        moveDirection.Normalize();

        float speed = WalkSpeed;
        if (sprintAction.ReadValue<float>() > 0)
        {
            speed *= SprintMultiplier;
        }

        characterController.Move(moveDirection * speed * Time.deltaTime);

        // Handle jumping
        if (jumpAction.triggered && IsGrounded())
        {
            velocity.y = JumpForce;
        }
        else
        {
            velocity.y += Gravity * Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);

        // Get look inputs (mouse)
        if (PlayerCamera != null)
        {
            Vector2 lookInput = lookAction.ReadValue<Vector2>();
            float mouseX = lookInput.x * LookSensitivityx;
            float mouseY = lookInput.y * LookSensitivityy;

            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, MinYLookAngle, MaxYLookAngle);

            PlayerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }

       
        if (transform.position.y < -5) 
        {
            ResetPlayer();
        }
    }

    bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, GroundCheckDistance))
        {
            return true;
        }
        return false;
    }

    private void ResetPlayer()
    {
        transform.position = StartingPosition; // Reset to starting position
        velocity = Vector3.zero; // Reset velocity
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player reaches the winning area
        if (other.CompareTag("Goal"))
        {
            WinText.gameObject.SetActive(true); // Show the win text
        }
    }
}
