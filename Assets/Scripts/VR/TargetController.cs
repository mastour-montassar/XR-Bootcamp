using System;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator;

    public Transform InitialTransform;  // Transform for less precise hits
    public Transform targetCenter;      // Transform for precise hits

    // Reference to the GunController
    private GunController gunController;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rb.isKinematic = true; // Initially, the target won't fall
    }

    // Set the GunController reference
    public void SetGunController(GunController controller)
    {
        gunController = controller;
    }

    // Call this method when the target is hit
    public void Hit(Vector3 hitPoint)
    {
        if (animator != null)
        {
            animator.enabled = false;
        }

        rb.isKinematic = false; // Allow the target to fall
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse); // Apply downward force

        // Determine score based on hit point
        int points = CalculatePoints(hitPoint);

        // Call the method in GunController to update the score
        if (gunController != null)
        {
            gunController.AddPoints(points);
        }
    }

    private int CalculatePoints(Vector3 hitPoint)
    {
        // Distance to InitialTransform
        float distanceToInitial = Vector3.Distance(hitPoint, InitialTransform.position);

        // Distance to targetCenter
        float distanceToCenter = Vector3.Distance(hitPoint, targetCenter.position);

        // Award points based on the closer transform
        if (distanceToCenter < distanceToInitial)
        {
            Debug.Log("Hit near center! Points: 10");
            return 10; // Precise hit
        }
        else
        {
            Debug.Log("Hit near outer area! Points: 5");
            return 5; // Less precise hit
        }
    }

    public void Reset()
    {
        transform.position = InitialTransform.position;
        transform.rotation = Quaternion.Euler(4.29491425f, 0f, 0f);
        rb.isKinematic = true;

        if (animator != null)
        {
            animator.enabled = true; // Restart animation
        }
    }
}
