using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class GunController : MonoBehaviour
{
    public Transform shootPoint; // Point where the projectile is spawned
    public float shootRange = 50f; // Range of the shot
    public List<TargetController> targets; 
    public int score = 0; // Score variable
    public TextMeshProUGUI  scoreText; // Reference to a UI Text to display the score
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
                // Call the Hit method on the target to make it fall
                
                target.Hit();
                score += 10;
                UpdateScoreText();
                if (score == 20)
                {
                    StartCoroutine(ResetAfterDelay());
                }
            }
        }
    }
 
    private IEnumerator ResetAfterDelay()
    {
        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);
        
        foreach (TargetController target in targets)
        {
            target.Reset(target.transform);
        }
        score = 0;
        UpdateScoreText();
        
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = " " + score;
        }
    }
    public void DisableHandRigidbody(Rigidbody handRigidbody)
    {
        if (handRigidbody != null)
        {
            handRigidbody.isKinematic = true;
        }
    }

    public void EnableHandRigidbody(Rigidbody handRigidbody)
    {
        if (handRigidbody != null)
        {
            handRigidbody.isKinematic = false;
        }
    }
}