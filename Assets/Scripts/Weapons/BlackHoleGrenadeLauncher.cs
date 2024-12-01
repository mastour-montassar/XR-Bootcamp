using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit;

namespace Weapons
{
    public class BlackHoleGrenadeLauncher : ShockPistol
    {
        [SerializeField] private LineRenderer trajectoryLineRenderer;  // Trajectory line
        [SerializeField] private GameObject blackHoleGrenadePrefab;    // Grenade prefab
        [SerializeField] private float firingForce = 5f;               // Grenade firing force
        [SerializeField] private int trajectoryPoints = 30;            // Number of points to visualize in the trajectory
        [SerializeField] private float trajectoryStep = 0.1f;          // Time step between each trajectory point
        [SerializeField] private float bounceDamping = 0.6f;           // Damping factor for bounce (velocity reduction after bounce)
        [SerializeField] private float gravityMultiplier = 1f;         // Multiplier to adjust gravity effect

        private bool isGunHeld = false;  // Tracks if the gun is being held

        protected override void Start()
        {
            base.Start();

            // Validate that the trajectoryLineRenderer is assigned
            Assert.IsNotNull(trajectoryLineRenderer, "LineRenderer not assigned for trajectory visualization.");

            // Subscribe to grab events
            _grabInteractable.selectEntered.AddListener(OnGunGrabbed);
            _grabInteractable.selectExited.AddListener(OnGunDropped);

            // Ensure the line renderer is initially disabled
            trajectoryLineRenderer.enabled = false;
        }

        private void Update()
        {
            // Update trajectory visualization only if the gun is being held
            if (isGunHeld)
            {
                VisualizeTrajectory();
            }
        }

        private void OnGunGrabbed(SelectEnterEventArgs args)
        {
            // Show the trajectory visualization
            isGunHeld = true;
            trajectoryLineRenderer.enabled = true;
        }

        private void OnGunDropped(SelectExitEventArgs args)
        {
            // Hide the trajectory visualization
            isGunHeld = false;
            trajectoryLineRenderer.enabled = false;
        }

        protected override void Fire(ActivateEventArgs arg0)
        {
            if (!CanFire()) return;

            // Instantiate grenade
            var grenadeObject = Instantiate(blackHoleGrenadePrefab, _gunBarrel.position, _gunBarrel.rotation);
            var grenadeRigidbody = grenadeObject.GetComponent<Rigidbody>();

            if (grenadeRigidbody == null)
            {
                Debug.LogError("The Black Hole Grenade prefab is missing a Rigidbody component!");
                return;
            }

            // Calculate and validate the firing direction
            Vector3 firingDirection = _gunBarrel.forward * firingForce;
            if (firingDirection == Vector3.zero || float.IsNaN(firingDirection.x))
            {
                Debug.LogError("Invalid firing direction!");
                return;
            }

            // Apply force to the grenade
            grenadeRigidbody.AddForce(firingDirection, ForceMode.VelocityChange);
        }

        private void VisualizeTrajectory()
        {
            trajectoryLineRenderer.positionCount = 0;  // Reset the position count at the start

            Vector3 startPosition = _gunBarrel.position;
            Vector3 startVelocity = _gunBarrel.forward * firingForce;
            Vector3 currentPosition = startPosition;
            Vector3 currentVelocity = startVelocity;

            int pointIndex = 0; // We will use this index to set positions in the LineRenderer

            // This loop calculates and draws the trajectory points
            for (int i = 0; i < trajectoryPoints; i++)
            {
                // Calculate trajectory point based on velocity and gravity
                float time = i * trajectoryStep;
                currentPosition = startPosition + currentVelocity * time + Physics.gravity * (0.5f * time * time) * gravityMultiplier;

                // Raycast to simulate bounces using all layers (LayerMask set to -1 for all layers)
                RaycastHit hit;
                if (Physics.Raycast(currentPosition, currentVelocity.normalized, out hit, currentVelocity.magnitude * trajectoryStep, -1)) // -1 = all layers
                {
                    // If hit detected, reflect velocity and reduce speed (apply bounce)
                    currentVelocity = Vector3.Reflect(currentVelocity, hit.normal) * bounceDamping;

                    // If we've hit an object, we stop the trajectory
                    trajectoryLineRenderer.positionCount = pointIndex + 1;  // Ensure we don't exceed the line renderer's capacity
                    trajectoryLineRenderer.SetPosition(pointIndex, hit.point);
                    pointIndex++; // Increment position index

                    // After bounce, continue the visualization from the hit point
                    startPosition = hit.point;

                    // Continue with the next trajectory point
                    continue;
                }

                // If no bounce occurs, just set the current position
                trajectoryLineRenderer.positionCount = pointIndex + 1;  // Adjust count to include the current point
                trajectoryLineRenderer.SetPosition(pointIndex, currentPosition);
                pointIndex++; // Increment position index

                // Stop the trajectory if the velocity is very small or the object hits the ground
                if (currentVelocity.magnitude < 0.1f) break;
            }
        }
    }
}
