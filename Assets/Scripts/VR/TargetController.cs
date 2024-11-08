using System;
using System.Collections;
using System.Security.Policy;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator;
    public Transform InitialTransform;
    private Transform initialPosition;

    void Awake()
    {
        initialPosition=InitialTransform;
        Debug.Log(initialPosition);
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rb.isKinematic = true; // Initially, the target won't fall
    }

   
    // Call this method to make the target fall
    public void Hit()
    {
        if (animator != null)
        {
            animator.enabled = false;
        }
        rb.isKinematic = false;  // Allow the target to fall
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);  // Apply a downward force to knock the target down
    }

    public void Reset(Transform transformPosition)
    {
        
        transform.position = InitialTransform.position;
        transform.rotation = Quaternion.Euler(4.29491425f, 0f, 0f);
        rb.isKinematic = true;

        if (animator != null)
        {
            animator.enabled = true; // Restart animation for reset
        }
        
        
    }
}
