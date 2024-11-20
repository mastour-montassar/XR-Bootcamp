using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AxeRecall : MonoBehaviour
{
    public Transform playerHand;  // Hand position for the axe to return to
    public float recallSpeed = 10f;  // Speed of recalling the axe
    private bool isThrown = false;  // State if the axe has been thrown
    private bool isRecalling = false;  // State if the axe is currently being recalled
    private Rigidbody rb;

    private XRGrabInteractable grabInteractable;  // Reference to the XR Grab Interactable

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

    }


    // Function to be triggered when the axe is grabbed
    public void OnGrab()
    {
        isThrown = false;  // Reset the thrown state
        isRecalling = false;  // Stop recalling if it's being recalled
        rb.isKinematic = false;  // Enable physics
    }

    // Function to be triggered when the axe is released (thrown)
    public void OnThrow(SelectExitEventArgs args)
    {
        // Get the transform of the interactor (controller or hand) that threw the axe
        var interactorTransform = args.interactor.transform;

        // Trigger throw action with added force
        rb.isKinematic = false;
        rb.AddForce(interactorTransform.forward * 10f, ForceMode.VelocityChange);
        rb.AddTorque(transform.right * 500f);  // Spin the axe
        isThrown = true;
    }





}
