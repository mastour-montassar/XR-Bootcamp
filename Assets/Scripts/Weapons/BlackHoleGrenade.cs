using UnityEngine;

public class BlackHoleGrenade : MonoBehaviour
{
    [Header("Black Hole Properties")]
    [SerializeField] private float pullRadius = 5f;         // Radius of the black hole effect
    [SerializeField] private float pullForce = 10f;         // Force pulling objects toward the grenade
    [SerializeField] private float pullDuration = 2f;       // Duration of the pulling effect
    [SerializeField] private float explosionForce = 20f;    // Force of the explosion
    [SerializeField] private float explosionRadius = 7f;    // Radius of the explosion effect
    [SerializeField] private LayerMask affectedLayers;      // Layers affected by the black hole
    [SerializeField] private GameObject grenadeVisuals;     // Reference to the grenade's visuals (e.g., mesh or particles)
    [SerializeField] private ParticleSystem explosionEffect; // Optional: Explosion effect particle system

    private bool isPulling = true;

    private void Start()
    {
        // Start pulling objects
        Invoke(nameof(PrepareExplosion), pullDuration); // Prepare explosion after pulling ends
    }

    private void FixedUpdate()
    {
        if (!isPulling) return;

        // Detect objects within the pull radius
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, pullRadius, affectedLayers);

        foreach (var obj in objectsInRange)
        {
            // Ensure the object has a Rigidbody
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Pull objects toward the grenade
                Vector3 pullDirection = (transform.position - rb.position).normalized;
                rb.AddForce(pullDirection * pullForce * Time.fixedDeltaTime, ForceMode.Acceleration);
            }
        }
    }

    private void PrepareExplosion()
    {
        isPulling = false; // Stop pulling objects

        // Hide the grenade visuals
        if (grenadeVisuals != null)
        {
            grenadeVisuals.SetActive(false);
        }

        // Optional: Play explosion particle effect
        if (explosionEffect != null)
        {
            explosionEffect.Play();
        }

        // Delay to apply the explosive force to give time for the grenade disappearance effect
        Invoke(nameof(TriggerExplosion), 0.5f); // Add a small delay (e.g., 0.5 seconds)
    }

    private void TriggerExplosion()
    {
        // Detect objects within the explosion radius
        Collider[] objectsInExplosionRange = Physics.OverlapSphere(transform.position, explosionRadius, affectedLayers);

        foreach (var obj in objectsInExplosionRange)
        {
            // Ensure the object has a Rigidbody
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Apply an outward explosion force
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f, ForceMode.Impulse);
            }
        }

        // Destroy the grenade after the explosion
        Destroy(gameObject, 0.1f);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize pull radius
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pullRadius);

        // Visualize explosion radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
