using UnityEngine;
using UnityEngine.InputSystem;

public class ShipController : MonoBehaviour
{
    public GameObject ship;                 // Reference to the spaceship
    public ControlVolume controlVolume;   
    [SerializeField] private Transform handTransform; // Reference to the control volume script
    [SerializeField] private float movementSpeed = 1.5f;  // Speed multiplier for ship movement
    [SerializeField] private float rotationSpeed = 0.03f;
    public InputActionAsset inputActions;   // Input Action Asset reference
    private InputAction controlStartAction; // The action for starting control
    private bool isControllingShip = false; // Track if control is active
    private Vector3 initialHandPosition;  
    private Quaternion initialHandRotation;
    void OnEnable()
    {
        // Find the ControlStartAction within the input asset
        controlStartAction = inputActions.FindActionMap("XRI RightHand").FindAction("shipcontrol");

        // Subscribe to the action event
        controlStartAction.performed += OnControlStart;
        controlStartAction.Enable();
    }

    void OnDisable()
    {
        // Unsubscribe when disabling
        controlStartAction.performed -= OnControlStart;
        controlStartAction.Disable();
    }

    void OnControlStart(InputAction.CallbackContext context)
    {
        if (controlVolume.IsHandInside())
        {
            isControllingShip = true;
            initialHandPosition = controlVolume.transform.localPosition;
            initialHandRotation = handTransform.rotation;  
            Debug.Log("Control initiated");
        }
    }

    void Update()
    {
        if (!controlVolume.IsHandInside() && isControllingShip)
        {
            isControllingShip = false;
            Debug.Log("Control stopped");
        }

        if (isControllingShip)
        {
            ControlShip();
        }
    }

    void ControlShip()  
    {        // Get the current hand position and rotation
        Vector3 currentHandPosition = handTransform.position;
        Quaternion currentHandRotation = handTransform.rotation;

        // Calculate hand movement relative to initial position (forward/backward)
        float handMovementZ = currentHandPosition.z - initialHandPosition.z;
        Vector3 movement = Vector3.forward * (handMovementZ * movementSpeed * Time.deltaTime);
        ship.transform.Translate(movement, Space.Self);  // Move the ship forward/backward

        // Calculate hand movement for side-to-side (left/right) translation (optional)
        float handMovementX = currentHandPosition.x - initialHandPosition.x;
        Vector3 lateralMovement = Vector3.right * (handMovementX * movementSpeed * Time.deltaTime);
        ship.transform.Translate(lateralMovement, Space.Self);  // Move the ship left/right

        // Calculate hand movement for up/down translation (optional)
        float handMovementY = currentHandPosition.y - initialHandPosition.y;
        Vector3 verticalMovement = Vector3.up * (handMovementY * movementSpeed * Time.deltaTime);
        ship.transform.Translate(verticalMovement, Space.Self);  // Move the ship up/down

        // Calculate hand rotation relative to the initial rotation
        Quaternion handRotationDelta = currentHandRotation * Quaternion.Inverse(initialHandRotation);

        // Apply rotation to the ship based on hand rotation (yaw, pitch, roll)
        Vector3 rotationEulerAngles = handRotationDelta.eulerAngles;
        ship.transform.Rotate(Vector3.up, rotationEulerAngles.y * rotationSpeed * Time.deltaTime, Space.Self);   // Yaw rotation (left/right)
        ship.transform.Rotate(Vector3.right, rotationEulerAngles.x * rotationSpeed * Time.deltaTime, Space.Self); // Pitch rotation (up/down)
        ship.transform.Rotate(Vector3.forward, rotationEulerAngles.z * rotationSpeed * Time.deltaTime, Space.Self); // Roll rotation (twist)
    }
 }

