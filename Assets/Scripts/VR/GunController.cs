using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunController : MonoBehaviour
{
    public Transform shootPoint; // Point where the projectile is spawned
    public float shootRange = 50f; // Range of the shot
    public List<TargetController> targets; // List of targets to reset
    public int score = 0; // Current score
    public TextMeshProUGUI scoreText; // UI Text to display the score

    private int targetsHit = 0; // Track how many targets have been hit

    void Start()
    {
        // Ensure all targets know about the GunController
        foreach (TargetController target in targets)
        {
            target.SetGunController(this);
        }
    }

    public void Shoot()
    {
        // Cast a ray from the shootPoint forward
        RaycastHit hit;
        if (Physics.Raycast(shootPoint.position, shootPoint.forward, out hit, shootRange))
        {
            // Check if the ray hits a target (check for "TargetController")
            TargetController target = hit.collider.GetComponent<TargetController>();
            if (target != null)
            {
                // Pass the hit point to the target's Hit method
                target.Hit(hit.point);

                // Increase the targetsHit counter when a target is hit
                targetsHit++;

                // Check if all targets have been hit
                if (targetsHit == targets.Count)
                {
                    StartCoroutine(ResetAfterDelay());
                }
            }
        }
    }

    public void AddPoints(int points)
    {
        // Debug to see if AddPoints is triggered correctly
        Debug.Log($"AddPoints called with {points} points.");

        // Add points to the total score
        score += points;

        // Update the score text
        UpdateScoreText();
    }

    private IEnumerator ResetAfterDelay()
    {
        // Wait for 3 seconds before resetting the targets
        yield return new WaitForSeconds(3f);

        // Reset all targets
        foreach (TargetController target in targets)
        {
            target.Reset();
        }

        // Reset score
        score = 0;
        UpdateScoreText();

        // Reset the hit counter
        targetsHit = 0;
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"{score}";
        }
    }
}
