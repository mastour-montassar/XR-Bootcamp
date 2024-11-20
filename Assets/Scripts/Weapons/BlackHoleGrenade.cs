using UnityEngine;

namespace Weapons
{
    public class BlackHoleGrenade : MonoBehaviour
    {
        public float pullRadius = 10f;  // Radius of the black hole effect
        public float pullForce = 50f;   // Strength of the pull
        public float teleportDistance = 20f; // Teleport distance for objects
        public float explosionDelay = 3f; // Delay before explosion
        private bool exploded = false;

        void Start()
        {
            Destroy(gameObject, explosionDelay);  // Destroy after delay
        }

        void Update()
        {
            if (!exploded)
            {
                // Pull objects towards the grenade
                Collider[] collidersInRange = Physics.OverlapSphere(transform.position, pullRadius);
                foreach (var collider in collidersInRange)
                {
                    Rigidbody rb = collider.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        // Pull force towards the grenade center
                        Vector3 direction = (transform.position - collider.transform.position).normalized;
                        rb.AddForce(direction * pullForce * Time.deltaTime);
                    }
                }
            }
        }

        void Explode()
        {
            // Teleport objects that are within the black hole's range
            Collider[] collidersInRange = Physics.OverlapSphere(transform.position, pullRadius);
            foreach (var collider in collidersInRange)
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 newLocation = transform.position + (transform.position - collider.transform.position).normalized * teleportDistance;
                    rb.position = newLocation;
                    rb.velocity = Vector3.zero;  // Stop any further movement
                }
            }

            exploded = true;
            Destroy(gameObject);  // Destroy grenade after explosion
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!exploded)
            {
                Explode();  // Trigger the explosion on collision
            }
        }
    }
}
