using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AxeRecall : MonoBehaviour
{
    public Transform playerHand;  // Hand position for the axe to return to
    public float recallSpeed = 10f;  // Speed of recalling the axe
    private bool isThrown = false;  // State if the axe has been thrown
    private bool isRecalling = false;  // State if the axe is currently being recalled
    private Rigidbody rb;
    private bool isEmbedded = false;  // State if the axe is embedded in an object

    private XRGrabInteractable grabInteractable;  // Reference to the XR Grab Interactable
    private Transform embeddedObject;  // The object where the axe is embedded

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
        isEmbedded = false;  // Reset embedding state
        embeddedObject = null;  // Reset the embedded object reference
        rb.isKinematic = false;  // Enable physics when grabbed
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

    // Function to handle collision with an object (wall or enemy)
    private void OnCollisionEnter(Collision collision)
    {
        if (isThrown && !isEmbedded)
        {
            // Check if the axe collides with an enemy or wall
            // You can further add checks to identify specific tags or layers for enemies

                // Embed the axe into the object by freezing its position and rotation
                rb.isKinematic = true;  // Disable physics (freeze position/rotation)
                transform.SetParent(collision.transform);  // Attach the axe to the collision object
                isEmbedded = true;  // Set the axe to be embedded
                embeddedObject = collision.transform;  // Save the reference of the object
            
        }
    }

    // Function to recall the axe back to the player
    public void ReturnAxe()
    {
        if (isEmbedded)
        {
            // Detach from the embedded object and re-enable physics
            transform.SetParent(null);  // Remove the axe from the parent object
            rb.isKinematic = false;  // Re-enable physics

            // Make the axe fly back to the player
            Vector3 directionToPlayer = (playerHand.position - transform.position).normalized;
            rb.velocity = directionToPlayer * recallSpeed;
            rb.angularVelocity = Vector3.zero;  // Stop the spinning effect while recalling
            isRecalling = true;  // Start recalling process
        }
    }

    // Function to update recall when axe is not embedded
    void Update()
    {
        if (isRecalling)
        {
            // Move axe back to the player's hand position smoothly
            transform.position = Vector3.Lerp(transform.position, playerHand.position, recallSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, playerHand.rotation, recallSpeed * Time.deltaTime);

            // Stop recalling once the axe is close enough to the player hand
            if (Vector3.Distance(transform.position, playerHand.position) < 0.1f)
            {
                isRecalling = false;  // Stop recalling when axe reaches the player
                rb.isKinematic = false;  // Re-enable physics for further interactions
            }
        }
    }
}
