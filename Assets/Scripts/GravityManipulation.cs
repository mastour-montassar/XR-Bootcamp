using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GravityManipulation : MonoBehaviour
{
    public enum GravityMode { Pull, Push }

    [SerializeField] private GravityMode mGravityMode = GravityMode.Pull;
    [SerializeField] private float gravityRadius = 5f;
    [SerializeField] private float gravityForce = 10f;
    [SerializeField] private float pullSpeed = 20f;
    [SerializeField] private float pushSpeed = 20f;
    [SerializeField] private float moveSpeed = 10f;

    private bool isGrabbed = false;
    private bool ispush = false;
    private bool ispull = false;
    private XRGrabInteractable grabInteractable;
    private static float nb =0;
    
    private List<Rigidbody> controlledObjects = new List<Rigidbody>(); // List to track controlled objects
    private Vector3 controllerDirection;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        controllerDirection = args.interactor.transform.forward;
        nb++;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        nb--;
        ispull = false;
        ispush = false;
        isGrabbed = false;
        ResetObjectsGravity(); // Activate gravity for controlled objects
    }

    private void FixedUpdate()
    {
        if (!isGrabbed) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, gravityRadius);

        foreach (var item in colliders)
        {
            // Check if the object has the tag "Cube"
            if (item.CompareTag("cube"))
            {
                Rigidbody rb = item.GetComponent<Rigidbody>();
                if (rb == null || rb.gameObject == this.gameObject) continue;

                if (!controlledObjects.Contains(rb))
                {
                    controlledObjects.Add(rb); // Add object to controlled list
                }

                // Set gravity mode based on object tag
                if (item.CompareTag("PullObject"))
                {
                    mGravityMode = GravityMode.Pull;
                    ispull = true;
                }
                else if (item.CompareTag("PushObject"))
                {
                    mGravityMode = GravityMode.Push;
                    ispush = true;
                }

                // Apply the pull or push effect and move with controller
                if (nb==2)
                {
                    PullObjectToPlunger(rb);
                    MoveObjectWithController(rb);
                }
                else
                {
                    switch (mGravityMode)
                    {
                        case GravityMode.Pull:
                            PullObjectToPlunger(rb);
                            break;
                        case GravityMode.Push:
                            if (!ispull)
                            {
                                PushObjectAway(rb);
                            }
                            break;
                    }
                }
                // Allow the object to move with the controller
               
            }
        }
    }

    private void PullObjectToPlunger(Rigidbody rb)
    {
        Vector3 direction = (transform.position - rb.position).normalized;
        rb.useGravity = false;
        rb.velocity = direction * pullSpeed;

        if (Vector3.Distance(transform.position, rb.position) < 0.5f)
        {
            rb.useGravity = true;
            rb.velocity = Vector3.zero;
        }
    }

    private void PushObjectAway(Rigidbody rb)
    {
        Vector3 direction = (rb.position - transform.position).normalized;
        rb.useGravity = false;
        rb.velocity = direction * pushSpeed;

        if (Vector3.Distance(transform.position, rb.position) > gravityRadius)
        {
            rb.useGravity = true;
            rb.velocity = Vector3.zero;
        }
    }

    private void MoveObjectWithController(Rigidbody rb)
    {
        controllerDirection = grabInteractable.selectingInteractor.transform.forward;
        Vector3 newPosition = rb.position + controllerDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    private void ResetObjectsGravity()
    {
        foreach (var rb in controlledObjects)
        {
            if (rb != null)
            {
                rb.useGravity = true; // Restore gravity
            }
        }
        controlledObjects.Clear(); // Clear the list after releasing
    }

    public void SetMode(GravityMode mode)
    {
        mGravityMode = mode;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isGrabbed ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, gravityRadius);
    }
}