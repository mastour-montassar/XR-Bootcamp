using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class GManger : MonoBehaviour

{
    

    
    public int maxThrows = 3; // Maximum throws allowed
    private int currentThrows = 0; // Tracks current throws

    // Reference to the CanManager to reset cans if needed
    private CanManager canManager;

    void Start()
    {
        // Get the CanManager instance
        canManager = CanManager.Instance;
    }

    public void RegisterThrow()
    {
        StartCoroutine(ResetSceneAfterDelay(3f));  // Pass in 3 seconds delay
        
    }

    private IEnumerator ResetSceneAfterDelay(float delay)
    {
        // Wait for the specified time before reloading the scene
        yield return new WaitForSeconds(delay);
        if (CanManager.win)
        {yield break;;}
        
        // Reloads the currently active scene
        currentThrows++;
        Debug.Log("Throw registered! Current throws: " + currentThrows);

        // Check if the player has used all their throws
        if (currentThrows >= maxThrows)
        {
            Debug.Log("Out of balls! Resetting cans.");
            canManager.ResetGame(); // Call the reset method from CanManager
            currentThrows = 0; // Reset throw count for a new round
        }
    }
    

}