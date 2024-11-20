using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GravityManipulation : MonoBehaviour
{
    public float pullForce = 10f;   // Strength of the pull force
    public float pushForce = 10f;   // Strength of the push force
    public float maxDistance = 10f; // Maximum distance at which forces can be applied

    private Collider[] collidersInRange; // To hold colliders of objects in range

    // Function to be called when the pull device is activated
    public void OnPullActivated(SelectEnterEventArgs args)
    {
        var interactor = args.interactor;
        if (interactor == null) return;

        Transform handTransform = interactor.transform;

        // Check for the objects in range of the plunger
        collidersInRange = Physics.OverlapSphere(handTransform.position, maxDistance);

        // Apply the pull force
        ApplyPullVisual(handTransform);
    }

    // Function to be called when the push device is activated
    public void OnPushActivated(SelectEnterEventArgs args)
    {
        var interactor = args.interactor;
        if (interactor == null) return;

        Transform handTransform = interactor.transform;

        // Check for the objects in range of the plunger
        collidersInRange = Physics.OverlapSphere(handTransform.position, maxDistance);

        // Apply the push force
        ApplyPushVisual(handTransform);
    }

    void ApplyPullVisual(Transform handTransform)
    {
        foreach (var collider in collidersInRange)
        {
            if (collider.CompareTag("cube")) // Check if the object has the "cube" tag
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Calculate the direction to pull the object
                    Vector3 direction = (handTransform.position - collider.transform.position).normalized;

                    // Apply a visual force to simulate pulling
                    rb.useGravity = false; // Disable gravity
                    rb.velocity = direction * pullForce; // Apply visual movement towards the hand (without kinematic)

                    // Optionally, you can add a small drag or dampening effect to smooth out the movement
                    rb.drag = 2f; // Adjust as necessary to smooth the movement
                }
            }
        }
    }

    void ApplyPushVisual(Transform handTransform)
    {
        foreach (var collider in collidersInRange)
        {
            if (collider.CompareTag("cube")) // Check if the object has the "cube" tag
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Calculate the direction to push the object
                    Vector3 direction = (collider.transform.position - handTransform.position).normalized;

                    // Apply a visual force to simulate pushing
                    rb.useGravity = false; // Disable gravity
                    rb.velocity = direction * pushForce; // Apply visual movement away from the hand (without kinematic)

                    // Optionally, you can add a small drag or dampening effect to smooth out the movement
                    rb.drag = 2f; // Adjust as necessary to smooth the movement
                }
            }
        }
    }
}
