using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class CanManager : MonoBehaviour
{
    
    public static CanManager Instance;

    public List<Transform> cans ;
    private int knockedDownCount = 0;
    private float yPositionThreshold = 0.4f;
    private int counter = 0;
    public static bool win = false;

    void Awake()
    {
        // Singleton pattern for easy access
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    

    void Update()
    {
        counter = 0;
        RegisterKnockedDownCan();
    }

    public void RegisterKnockedDownCan()
    {
        foreach (var can in cans)
        {
            if (can.position.y < yPositionThreshold)
            {
                counter++;
            }
            
        }
        if (counter==3 && win==false)
        {
            win = true;
            Debug.Log("All cans are knocked down! You win!");
            // Trigger reset or win logic
            ResetGame();
        }
    }
    
    public void ResetGame()
    {
        StartCoroutine(ResetSceneAfterDelay(3f));  // Pass in 3 seconds delay
        
    }

    private IEnumerator ResetSceneAfterDelay(float delay)
    {
        // Wait for the specified time before reloading the scene
        yield return new WaitForSeconds(delay);
        
        // Reloads the currently active scene
        win = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    
}