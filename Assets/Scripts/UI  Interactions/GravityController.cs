using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GravityController : MonoBehaviour
{
    public Button gravityButton;                // Reference to the button
    public List<Rigidbody> objectsToToggle;     // List of objects to apply gravity to
    private bool isGravityOn = false;            // Track gravity state (on/off)

    void Start()
    {
        // Add listener for the button click event
        gravityButton.onClick.AddListener(ToggleGravity);

        // Optionally, set the initial gravity state for all objects (gravity on)
        SetGravityState(isGravityOn);
    }

    void ToggleGravity()
    {
        // Toggle the gravity state
        isGravityOn = !isGravityOn;

        // Update the gravity state for each object in the list
        SetGravityState(isGravityOn);

        // Optionally log to confirm the gravity state change
        Debug.Log("Gravity Toggled: " + (isGravityOn ? "On" : "Off"));
    }

    // Function to set gravity for each object in the list
    void SetGravityState(bool gravityState)
    {
        foreach (Rigidbody rb in objectsToToggle)
        {
            if (rb != null)  // Ensure the object has a Rigidbody
            {
                rb.useGravity = gravityState;  // Enable or disable gravity
            }
        }
    }
}