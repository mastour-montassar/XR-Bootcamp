using UnityEngine;
using UnityEngine.InputSystem;

public class ShipController : MonoBehaviour
{
    public GameObject ship;                 // Reference to the spaceship
    public ControlVolume controlVolume;   
    [SerializeField] private Transform handTransform; // Reference to the control volume script
    [SerializeField] private float movementSpeed = 1.5f;  // Speed multiplier for ship movement
    [SerializeField] private float rotationSpeed = 0.03f; // Speed multiplier for ship rotation
    [SerializeField] private float movementThreshold = 0.1f; // Threshold for movement detection
    [SerializeField] private float movementMultiplier = 1000000f;  // Multiplier to extend movement distance (set to 1000)
    [SerializeField] private float rotationDamping = 0.1f;  // Damping factor for smooth rotation

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
    {
        // Get the current hand position and rotation
        Vector3 currentHandPosition = handTransform.position;
        Quaternion currentHandRotation = handTransform.rotation;

        // Calculate the difference in position (hand movement relative to the initial hand position)
        Vector3 handMovement = currentHandPosition - initialHandPosition;

        // Move the ship based on hand movement in X (left/right) and Z (forward/backward)
        if (Mathf.Abs(handMovement.x) > movementThreshold)
        {
            ship.transform.Translate(Vector3.right * handMovement.x * movementSpeed * movementMultiplier * Time.deltaTime, Space.Self);
            initialHandPosition.x = currentHandPosition.x; // Update initial hand position after movement
        }

        if (Mathf.Abs(handMovement.z) > movementThreshold)
        {
            ship.transform.Translate(Vector3.forward * handMovement.z * movementSpeed * movementMultiplier * Time.deltaTime, Space.Self);
            initialHandPosition.z = currentHandPosition.z; // Update initial hand position after movement
        }

        // Move the ship vertically if needed (handMovement.y), but you can omit this if unnecessary
        if (Mathf.Abs(handMovement.y) > movementThreshold)
        {
            ship.transform.Translate(Vector3.up * handMovement.y * movementSpeed * movementMultiplier * Time.deltaTime, Space.Self);
            initialHandPosition.y = currentHandPosition.y; // Update initial hand position after movement
        }

        // Calculate rotation based on hand rotation (simplified to yaw for left/right rotation)
        Quaternion handRotationDelta = currentHandRotation * Quaternion.Inverse(initialHandRotation);
        Vector3 rotationEulerAngles = handRotationDelta.eulerAngles;

        // Apply yaw rotation based on hand movement
        if (Mathf.Abs(rotationEulerAngles.y) > movementThreshold)
        {
            // Smoothing the rotation by interpolating (Lerp) towards the target rotation
            Quaternion targetRotation = Quaternion.Euler(0, rotationEulerAngles.y, 0);
            ship.transform.rotation = Quaternion.Slerp(ship.transform.rotation, targetRotation, rotationDamping * Time.deltaTime);
        }

        // Apply pitch rotation based on hand movement (optional)
        if (Mathf.Abs(rotationEulerAngles.x) > movementThreshold)
        {
            // Smooth pitch rotation (up/down) as well
            Quaternion targetPitchRotation = Quaternion.Euler(rotationEulerAngles.x, 0, 0);
            ship.transform.rotation = Quaternion.Slerp(ship.transform.rotation, targetPitchRotation, rotationDamping * Time.deltaTime);
        }

        // Apply roll rotation (optional)
        if (Mathf.Abs(rotationEulerAngles.z) > movementThreshold)
        {
            // Smooth roll rotation (twist) as well
            Quaternion targetRollRotation = Quaternion.Euler(0, 0, rotationEulerAngles.z);
            ship.transform.rotation = Quaternion.Slerp(ship.transform.rotation, targetRollRotation, rotationDamping * Time.deltaTime);
        }
    }
}
