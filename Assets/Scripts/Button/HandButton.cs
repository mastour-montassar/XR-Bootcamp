using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
public class HandButton : MonoBehaviour
{
    public bool isPressed = false;
    private bool showingEnemies = false;  // Track which mode is active (enemies or water)
    public Camera minimapCamera; 


    private void Start()
    {
        SwitchToEnemies();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        isPressed = true;
        // Ensure the button is only pressed by the left hand
       if (other.CompareTag("button")){
           
           Debug.Log(other.name);
            PressButton();
       }
        
 
    }
    private void SwitchToEnemies()
    {
        Debug.Log("Switching to Enemies view");

        // Enable the "Enemies" layer
        minimapCamera.cullingMask |= (1 << LayerMask.NameToLayer("Minimap"));  // Bitwise OR to enable layer
        

        // Disable the "Water" layer
        minimapCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Water"));   // Bitwise AND NOT to disable layer
    }

    // Switch to water view (activate Water, deactivate Enemies)
    private void SwitchToWater()
    {
        Debug.Log("Switching to Water view");

        // Enable the "Water" layer
        minimapCamera.cullingMask |= (1 << LayerMask.NameToLayer("Water"));    // Bitwise OR to enable layer
        // Disable the "Enemies" layer
        minimapCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Minimap")); // Bitwise AND NOT to disable layer
    }


    private void PressButton()
    {
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

        }
    }
}