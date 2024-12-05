using UnityEngine;

public class KinematicButton : MonoBehaviour
{
    public Rigidbody buttonRigidbody; // The Rigidbody of the button
    private bool isTriggered = false; // State to track if the trigger occurred

    void Start()
    {
        // Set the Rigidbody to kinematic at the start
        buttonRigidbody.isKinematic = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the trigger is caused by a specific object
        if (other.name == ("LeftHand")) // Replace "TriggerObject" with your desired tag
        {
            Debug.Log("Trigger detected! Making button non-kinematic.");
            isTriggered = true;

            // Make the button non-kinematic
            buttonRigidbody.isKinematic = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Optionally, reset the button to kinematic when the object exits the trigger
        if (other.name == ("LeftHand"))
        {
            Debug.Log("Trigger ended. Making button kinematic again.");
            isTriggered = false;

            // Make the button kinematic again
            buttonRigidbody.isKinematic = true;
        }
    }
}