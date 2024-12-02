using UnityEngine;
using UnityEngine.InputSystem;

public class ShipController : MonoBehaviour
{
    public GameObject ship;                 // Reference to the spaceship
    public ControlVolume controlVolume;   
    [SerializeField] private Transform handTransform; // Reference to the control volume script
    [SerializeField] private float movementSpeed = 0.5f;  // Speed multiplier for ship movement
    [SerializeField] private float rotationSpeed = 1000f; // Speed multiplier for ship rotation
    [SerializeField] private float movementThreshold = 0.1f; // Threshold for movement detection
    [SerializeField] private float movementMultiplier = 1000f;  // Multiplier for movement magnitude
    [SerializeField] private float movementSmoothingTime = 1000f; // Damping time for smooth transitions
    [SerializeField] private float maxMovementDistance = 2.0f; // Maximum movement distance from the starting point
    [SerializeField] private float rotationDamping = 0.1f;  // Damping factor for smooth rotation

    public InputActionAsset inputActions;   // Input Action Asset reference
    private InputAction controlStartAction; // The action for starting control
    private bool isControllingShip = false; // Track if control is active
    private Vector3 initialHandPosition;  
    private Quaternion initialHandRotation;
    private Vector3 shipStartPosition; // Ship's original position
    private Vector3 currentVelocity = Vector3.zero;  // Velocity reference for SmoothDamp

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
            initialHandPosition = handTransform.position; // Set initial hand position
            initialHandRotation = handTransform.rotation;  // Set initial hand rotation
            shipStartPosition = ship.transform.position;   // Store ship's start position
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
{
    // Get the current hand position and rotation
    Vector3 currentHandPosition = handTransform.position;
    Quaternion currentHandRotation = handTransform.rotation;

    // Calculate the difference in position (hand movement relative to the initial hand position)
    Vector3 handMovement = currentHandPosition - initialHandPosition;

    // Move the ship based on hand movement, but limit movement distance
    if (handMovement.magnitude > movementThreshold)
    {
        // Calculate the target position based on hand movement
        Vector3 targetPosition = shipStartPosition + handMovement * movementSpeed * movementMultiplier;

        // Clamp the target position to ensure the ship doesn't move beyond the max distance
        Vector3 clampedTargetPosition = Vector3.ClampMagnitude(targetPosition - shipStartPosition, maxMovementDistance) + shipStartPosition;

        // Use SmoothDamp to move the ship smoothly towards the clamped target position
        ship.transform.position = Vector3.SmoothDamp(ship.transform.position, clampedTargetPosition, ref currentVelocity, movementSmoothingTime);
    }

    // Rotation logic for yaw, pitch, and roll
    Quaternion handRotationDelta = currentHandRotation * Quaternion.Inverse(initialHandRotation);
    Vector3 rotationEulerAngles = handRotationDelta.eulerAngles;

    // Apply yaw rotation
    if (Mathf.Abs(rotationEulerAngles.y) > movementThreshold)
    {
        Quaternion targetRotation = Quaternion.Euler(0, rotationEulerAngles.y, 0);
        ship.transform.rotation = Quaternion.Slerp(ship.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // Apply pitch rotation (up/down)
    if (Mathf.Abs(rotationEulerAngles.x) > movementThreshold)
    {
        Quaternion targetPitchRotation = Quaternion.Euler(rotationEulerAngles.x, ship.transform.rotation.eulerAngles.y, ship.transform.rotation.eulerAngles.z);
        ship.transform.rotation = Quaternion.Slerp(ship.transform.rotation, targetPitchRotation, rotationSpeed * Time.deltaTime);
    }

    // Apply roll rotation
    if (Mathf.Abs(rotationEulerAngles.z) > movementThreshold)
    {
        Quaternion targetRollRotation = Quaternion.Euler(ship.transform.rotation.eulerAngles.x, ship.transform.rotation.eulerAngles.y, rotationEulerAngles.z);
        ship.transform.rotation = Quaternion.Slerp(ship.transform.rotation, targetRollRotation, rotationSpeed * Time.deltaTime);
    }
}

}
