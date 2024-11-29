using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GravityManipulation : MonoBehaviour
{
    // Enum for gravity modes
    public enum GravityMode { Pull, Push }

    [SerializeField] private GravityMode mGravityMode = GravityMode.Pull; // Current gravity mode
    [SerializeField] private float gravityRadius = 5f;   // Radius of effect
    [SerializeField] private float gravityForce = 10f;   // Strength of gravity force
    [SerializeField] private float pullSpeed = 20f;      // Speed for pulling objects
    [SerializeField] private float pushSpeed = 20f;      // Speed for pushing objects

    private bool isGrabbed = false; // State to track if the object is grabbed

    private XRGrabInteractable grabInteractable; // XRGrabInteractable component

    private void Awake()
    {
        // Get the XRGrabInteractable component
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Subscribe to grab events
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDestroy()
    {
        // Unsubscribe from grab events
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        ResetObjectsGravity();
    }

    private void FixedUpdate()
    {
        // Apply gravity forces only when the object is grabbed
        if (!isGrabbed) return;

        // Get all colliders within the gravity radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, gravityRadius);

        foreach (var item in colliders)
        {
            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb == null || rb.gameObject == this.gameObject) continue; // Skip if no Rigidbody or the grabbed object itself

            // Apply force based on the current gravity mode
            switch (mGravityMode)
            {
                case GravityMode.Pull:
                    PullObjectToPlunger(rb);
                    break;

                case GravityMode.Push:
                    PushObjectAway(rb);
                    break;

                default:
                    break;
            }
        }
    }

    private void PullObjectToPlunger(Rigidbody rb)
    {
        // Calculate the direction to pull the object towards the plunger
        Vector3 direction = (transform.position - rb.position).normalized;

        // Apply velocity to pull the object smoothly towards the plunger
        rb.useGravity = false; // Disable gravity temporarily
        rb.velocity = direction * pullSpeed;

        // Check if the object is close enough to stop pulling
        if (Vector3.Distance(transform.position, rb.position) < 0.5f)
        {
            rb.useGravity = true; // Restore gravity
            rb.velocity = Vector3.zero; // Stop movement
        }
    }

    private void PushObjectAway(Rigidbody rb)
    {
        // Calculate the direction to push the object away from the plunger
        Vector3 direction = (rb.position - transform.position).normalized;

        // Apply velocity to push the object smoothly away from the plunger
        rb.useGravity = false; // Disable gravity temporarily
        rb.velocity = direction * pushSpeed;

        // Check if the object is moving too far away (finish push)
        if (Vector3.Distance(transform.position, rb.position) > gravityRadius)
        {
            rb.useGravity = true; // Restore gravity once the push action ends
            rb.velocity = Vector3.zero; // Stop movement
        }
    }

    private void ResetObjectsGravity()
    {
        // Reset gravity for all objects within range when releasing the plunger
        Collider[] colliders = Physics.OverlapSphere(transform.position, gravityRadius);

        foreach (var item in colliders)
        {
            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true; // Re-enable gravity
                rb.velocity = Vector3.zero; // Stop any movement
                rb.angularVelocity = Vector3.zero; // Stop any rotation
            }
        }
    }

    public void SetMode(GravityMode mode)
    {
        mGravityMode = mode;
    }

    private void OnDrawGizmos()
    {
        // Draw the gravity radius for visualization in the Scene view
        Gizmos.color = isGrabbed ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, gravityRadius);
    }
}