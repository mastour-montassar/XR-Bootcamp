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
        
        protected override void Start()
        {
            base.Start();

            // Validate that the trajectoryLineRenderer is assigned
            Assert.IsNotNull(trajectoryLineRenderer, "LineRenderer not assigned for trajectory visualization.");

            // Subscribe to grab event
            _grabInteractable.selectEntered.AddListener(OnGunGrabbed);
            _grabInteractable.selectExited.AddListener(OnGunDropped);

            // Ensure the line renderer is initially disabled
            trajectoryLineRenderer.enabled = false;
        }

        private void OnGunGrabbed(SelectEnterEventArgs args)
        {
            // Show the trajectory visualization
            trajectoryLineRenderer.enabled = true;
            VisualizeTrajectory();
        }

        private void OnGunDropped(SelectExitEventArgs args)
        {
            // Hide the trajectory visualization
            trajectoryLineRenderer.enabled = false;
        }

        protected override void Fire(ActivateEventArgs arg0)
        {
            if (!CanFire()) return;

            base.Fire(arg0);  // Decrease ammo count or check ammo

            // Instantiate and launch the grenade
            var grenade = Instantiate(blackHoleGrenadePrefab, _gunBarrel.position, _gunBarrel.rotation)
                .GetComponent<Rigidbody>();
            grenade.AddForce(_gunBarrel.forward * firingForce, ForceMode.VelocityChange);

            // Update the trajectory visualization
            VisualizeTrajectory();
        }

        private void VisualizeTrajectory()
        {
            trajectoryLineRenderer.positionCount = trajectoryPoints;

            Vector3 startPosition = _gunBarrel.position;
            Vector3 startVelocity = _gunBarrel.forward * firingForce;

            for (int i = 0; i < trajectoryPoints; i++)
            {
                float time = i * trajectoryStep;
                Vector3 trajectoryPoint = startPosition + startVelocity * time + 0.5f * Physics.gravity * time * time;
                trajectoryLineRenderer.SetPosition(i, trajectoryPoint);
            }
        }
    }
}
