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
    public Transform toolHolder; 
    public float PickupRange = 5f; 
    public float Gravity = -9.8f;

    private Vector3 velocity;
    private float verticalRotation = 0f;
    private CharacterController characterController;
    private GameObject currentTool; 

 
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction lookAction;
    private InputAction pickupAction;
    private InputAction dropAction;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        // Set up input actions
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];
        lookAction = playerInput.actions["Look"];
        pickupAction = playerInput.actions["Pickup"]; 
        dropAction = playerInput.actions["Drop"]; 

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Movement
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

        // Handle camera look
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

        // Handle object pickup
        if (pickupAction.triggered)
        {
            TryPickupObject();
        }

        // Handle object drop
        if (dropAction.triggered)
        {
            DropTool();
        }

        // Position the held tool in front of the player
        if (currentTool != null)
        {
            currentTool.transform.position = toolHolder.position;
            currentTool.transform.rotation = toolHolder.rotation;
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

    // Try to pick up an object using a raycast
    void TryPickupObject()
    {
        Ray ray = new Ray(PlayerCamera.position, PlayerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, PickupRange))
        {
            if (hit.transform.CompareTag("Pickup")) 
            {
                EquipTool(hit.transform.gameObject);
            }
        }
    }

    // Equip the tool by attaching it to the tool holder
    void EquipTool(GameObject tool)
    {
        currentTool = tool;
        tool.GetComponent<Rigidbody>().isKinematic = true; // Disable physics while holding
        tool.transform.SetParent(toolHolder);
    }

    // Drop the tool and re-enable physics
    void DropTool()
    {
        if (currentTool != null)
        {
            currentTool.transform.SetParent(null); // Detach from the player
            currentTool.GetComponent<Rigidbody>().isKinematic = false; // Re-enable physics
            currentTool = null; // Clear the reference
        }
    }
}
