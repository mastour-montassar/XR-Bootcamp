using UnityEngine;

public class StabilizeButton : MonoBehaviour
{
    public Transform handTransform; // Make the hand transform public
    public Rigidbody buttonRigidbody;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    // Variables to track hand position for checking movement across all axes
    private Vector3 previousPosition;
    private float movementThreshold = 0.1f; // Threshold for movement detection

    void Start()
    {
        // Initialize initial position and rotation of the button
        initialPosition = buttonRigidbody.transform.localPosition;
        initialRotation = buttonRigidbody.transform.localRotation;

        // Initialize the previous position of the hand
        previousPosition = handTransform.localPosition;
    }

    void FixedUpdate()
    {
        // Calculate the total movement by checking the distance between the current and previous position
        float distanceMoved = Vector3.Distance(handTransform.localPosition, previousPosition);

        // If the hand has moved more than the threshold in any direction, stop stabilizing
        if (distanceMoved > movementThreshold)
        {
            // Allow the hand's position and rotation to change (no stabilization)
            previousPosition = handTransform.localPosition; // Update previous position to current hand position
        }
        else
        {
            // Stabilize the button's position and rotation if the hand is stable (below the threshold)
            buttonRigidbody.transform.localPosition = initialPosition;
            buttonRigidbody.transform.localRotation = initialRotation;
        }

        // Update previous position for the next FixedUpdate cycle
        previousPosition = handTransform.localPosition;
    }
}