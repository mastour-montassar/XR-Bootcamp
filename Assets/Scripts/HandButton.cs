using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
public class HandButton : MonoBehaviour
{
    private bool isPressed = false;
    private bool showingEnemies = false;  // Track which mode is active (enemies or water)
    public float pressDelay = 1.0f; 
    public Camera minimapCamera; 
    public Camera Camera;

    private Animator buttonAnimator;  // Reference to Animator

    private void Start()
    {

        // Get the Animator component from the button
        buttonAnimator = GetComponent<Animator>();
        SwitchToEnemies();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ensure the button is only pressed by the left hand
        if (other.gameObject.name != "RightHand Controller"&& !isPressed) 
        {
            isPressed = true;
            PressButton();
        }
        
 
    }
    private void SwitchToEnemies()
    {
        Debug.Log("Switching to Enemies view");

        // Enable the "Enemies" layer
        minimapCamera.cullingMask |= (1 << LayerMask.NameToLayer("Minimap"));  // Bitwise OR to enable layer
        
        Camera.cullingMask |= (1 << LayerMask.NameToLayer("Minimap"));  // Bitwise OR to enable layer

        // Disable the "Water" layer
        minimapCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Water"));   // Bitwise AND NOT to disable layer
        Camera.cullingMask &= ~(1 << LayerMask.NameToLayer("Water"));   // Bitwise AND NOT to disable layer
    }

    // Switch to water view (activate Water, deactivate Enemies)
    private void SwitchToWater()
    {
        Debug.Log("Switching to Water view");

        // Enable the "Water" layer
        minimapCamera.cullingMask |= (1 << LayerMask.NameToLayer("Water"));    // Bitwise OR to enable layer
        Camera.cullingMask |= (1 << LayerMask.NameToLayer("Water")); 
        // Disable the "Enemies" layer
        minimapCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Minimap")); // Bitwise AND NOT to disable layer
        Camera.cullingMask &= ~(1 << LayerMask.NameToLayer("Minimap")); 
    }


    private void PressButton()
    {
        
        // Trigger the animation when the button is pressed
        if (buttonAnimator != null)
        {
            Debug.Log("Button Pressed by Hand!");
            buttonAnimator.SetTrigger("ButtonPressed");
        }
        StartCoroutine(SwitchRadarViewWithDelay());

    }
    
    private IEnumerator SwitchRadarViewWithDelay()
    {
        yield return new WaitForSeconds(pressDelay);  // Wait for the specified delay

        // Switch the radar view between enemies and water
        if (showingEnemies)
        {
            SwitchToWater();
        }
        else
        {
            SwitchToEnemies();
        }

        showingEnemies = !showingEnemies;  // Toggle the state
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<XRBaseInteractor>())
        {
            isPressed = false;
            
        if (buttonAnimator != null)
        {
            buttonAnimator.SetTrigger("ButtonOff");
        }

        }
    }
}