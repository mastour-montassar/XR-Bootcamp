using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
    public float detectionRange = 5f;       
    public GameObject explosionEffect;     

    void Update()
    {
        
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Robot"))
            {
                Explod(hit.gameObject);
                break; 
            }
        }
    }

    void Explod(GameObject robot)
    {
        // Instantiate explosion effect at mine's position
        GameObject explosionInstance = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(explosionInstance, 1f); // 1f = 1 second
        
        EnemyController robotExplode = robot.GetComponent<EnemyController>();

        if (robotExplode != null)
        {
            robotExplode.TriggerExplosion();
        }

 

        Destroy(gameObject);

        Debug.Log("Robot destroyed and mine exploded.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
