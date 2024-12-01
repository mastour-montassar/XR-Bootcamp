using UnityEngine;

public class BlackHoleGrenade : MonoBehaviour
{
    [Header("Black Hole Properties")]
    [SerializeField] private float pullRadius = 5f;          // Radius of the black hole effect
    [SerializeField] private float pullForce = 50000000000f;        // Strong force for pulling
    [SerializeField] private float teleportOffset = 2f;      // Range for random teleportation
    [SerializeField] private float stabilityThreshold = 0.1f;// Threshold speed below which the grenade is considered stable
    [SerializeField] private float pullDuration = 2f;        // Duration of the pulling effect (1 second)
    [SerializeField] private LayerMask affectedLayers;       // Layers affected by the black hole
    [SerializeField] private GameObject grenadeVisuals;      // Reference to the grenade's visuals (e.g., mesh or particles)
    [SerializeField] private ParticleSystem explosionEffect; // Optional: Explosion effect particle system
    [SerializeField] private float stabilityTime = 2f;       // Time after which the grenade will be considered stable if it's not already

    private bool isPulling = false;   // Pulling state
    private bool isStable = false;    // Stability state
    private Rigidbody grenadeRb;      // Rigidbody of the grenade
    private float stabilityTimer;     // Timer to track how long since the grenade was shot

    private void Start()
    {
        // Get the grenade's Rigidbody component for stability check
        grenadeRb = GetComponent<Rigidbody>();
        stabilityTimer = 0f; // Initialize the timer to 0
    }

    private void Update()
    {
        // Increment the stability timer
        stabilityTimer += Time.deltaTime;

        // Check if the grenade has become stable (velocity drops below threshold) or after stabilityTime
        if (!isStable && (grenadeRb.velocity.magnitude < stabilityThreshold || stabilityTimer > stabilityTime))
        {
            // Grenade is stable now
            isStable = true;

            // Make the grenade invisible first when stable
            if (grenadeVisuals != null)
            {
                grenadeVisuals.SetActive(false); // Make the grenade invisible when stable
            }

            // Immediately start pulling objects (only after stability is achieved)
            StartPulling();
        }
    }

    private void StartPulling()
    {
        // Wait for a moment after invisibility to make sure everything is stable, then begin pulling
        Invoke(nameof(ActivatePulling), 0.1f); // Delay for a brief moment after invisibility to ensure stability
    }

    private void ActivatePulling()
    {
        isPulling = true; // Begin the pulling phase

        // Start pulling for the defined pull duration (1 second)
        Invoke(nameof(StopPulling), pullDuration);
    }

    private void FixedUpdate()
    {
        // Ensure pulling only happens if the grenade is stable, invisible, and pulling is active
        if (!isStable || !isPulling) return;

        // Detect objects within the pull radius
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, pullRadius, affectedLayers);

        foreach (var obj in objectsInRange)
        {
            // Ensure the object has a Rigidbody and is not kinematic
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null && !rb.isKinematic)
            {
                // Pull objects toward the grenade
                Vector3 pullDirection = (transform.position - rb.position).normalized;
                rb.AddForce(pullDirection * pullForce * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }
        }
    }

    private void StopPulling()
    {
        // Stop pulling objects
        isPulling = false;

        // Optional: Play particle effect (can be used to signal the end of the pull)
        if (explosionEffect != null)
        {
            explosionEffect.Play();
        }

        // Trigger teleportation after pulling ends
        TeleportNearbyObjects();
    }

    private void TeleportNearbyObjects()
    {
        // Detect objects within the teleport radius
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, pullRadius, affectedLayers);

        foreach (var obj in objectsInRange)
        {
            // Ensure the object has a Rigidbody and is not kinematic
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null && !rb.isKinematic)
            {
                // Teleport objects to a nearby random position instead of applying explosion force
                Vector3 randomOffset = Random.insideUnitSphere * teleportOffset;  // Random offset within the teleport range
                Vector3 teleportPosition = rb.position + randomOffset;  // New teleport destination

                // Ensure the teleport position is valid (e.g., not underground)
                teleportPosition.y = Mathf.Max(teleportPosition.y, 0.5f); // Adjust based on environment (e.g., terrain height)

                rb.position = teleportPosition;  // Set the new position

                // Activate useGravity for the object after teleportation
                rb.useGravity = true;
            }
        }

        // Destroy the grenade after the teleportation
        Destroy(gameObject, 0.1f);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize pull radius
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pullRadius);
    }
}
