using UnityEngine;
using UnityEngine.InputSystem;

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

        Cursor.lockState = CursorLockMode.Locked;
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
}
