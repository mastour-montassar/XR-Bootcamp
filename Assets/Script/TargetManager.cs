using System.Collections;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    public GameObject[] targets; // Array of target GameObjects
    public float initialDelay = 2.0f; // Time before the first target activates
    public float initialInterval = 2.0f; // Interval between activations
    public float intervalDecrease = 0.1f; // How much to decrease the interval over time
    public float minimumInterval = 0.5f; // Minimum allowed interval

    private float currentInterval;
    private TargetBehavior currentlyActiveTarget = null; // Track the currently active target

    private void Start()
    {
        currentInterval = initialInterval;
        StartCoroutine(ActivateTargets());
    }

    private IEnumerator ActivateTargets()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            // Deactivate the currently active target, if any
            if (currentlyActiveTarget != null)
            {
                currentlyActiveTarget.SetInactive();
                currentlyActiveTarget = null;
            }

            // Choose a random target
            int randomIndex = Random.Range(0, targets.Length);
            TargetBehavior targetBehavior = targets[randomIndex].GetComponent<TargetBehavior>();

            // Light up the chosen target
            if (targetBehavior != null && !targetBehavior.isActive)
            {
                targetBehavior.LightUp();
                currentlyActiveTarget = targetBehavior; // Update the currently active target
            }

            yield return new WaitForSeconds(currentInterval);

            // Gradually decrease the interval to make the game harder
            if (currentInterval > minimumInterval)
            {
                currentInterval -= intervalDecrease;
            }
        }
    }
}