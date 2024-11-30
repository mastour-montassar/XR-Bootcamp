using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit;

namespace Weapons
{
    public class BlackHoleGrenadeLauncher : ShockPistol
    {
        [SerializeField] private LineRenderer trajectoryLineRenderer;  // Trajectory line
        [SerializeField] private GameObject blackHoleGrenadePrefab;    // Grenade prefab
        [SerializeField] private float firingForce = 15f;             // Grenade firing force
        [SerializeField] private int trajectoryPoints = 30;           // Number of points to visualize in the trajectory
        [SerializeField] private float trajectoryStep = 0.1f;         // Time step between each trajectory point
        
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
            trajectoryLineRenderer.positionCount = trajectoryPoints;

            Vector3 startPosition = _gunBarrel.position;
            Vector3 startVelocity = _gunBarrel.forward * firingForce;

            for (int i = 0; i < trajectoryPoints; i++)
            {
                float time = i * trajectoryStep;
                Vector3 trajectoryPoint = startPosition + startVelocity * time + Physics.gravity * (0.5f * time * time);

                // Optionally stop if a collision point is detected
                if (Physics.Raycast(startPosition, startVelocity.normalized, out RaycastHit hit, startVelocity.magnitude * trajectoryStep))
                {
                    trajectoryLineRenderer.positionCount = i + 1;
                    trajectoryLineRenderer.SetPosition(i, hit.point);
                    break;
                }

                trajectoryLineRenderer.SetPosition(i, trajectoryPoint);
            }
        }
    }
}
