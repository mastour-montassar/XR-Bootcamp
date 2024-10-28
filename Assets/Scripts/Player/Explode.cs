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
                ExplodeAndTriggerRagdoll(hit.gameObject);
                break;
            }
        }
    }

    void ExplodeAndTriggerRagdoll(GameObject robot)
    {
        GameObject explosionInstance = Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Destroy(explosionInstance, 1f);
        Destroy(gameObject);

        // Stop patrol and trigger ragdoll
        EnemyController enemyController = robot.GetComponent<EnemyController>();
        RobotRagdollController ragdollController = robot.GetComponent<RobotRagdollController>();

        if (enemyController != null)
        {
            enemyController.DisablePatrol(); // Stop patrol
        }

        if (ragdollController != null)
        {
            ragdollController.TriggerRagdoll(); // Enable ragdoll physics
        }

        Debug.Log("Robot patrol stopped and ragdoll triggered.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}