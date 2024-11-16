using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class SmoothTeleportationAnchor : TeleportationAnchor
{
    public float transitionSpeed = 2f; 
    private Volume postProcessingVolume;
    private Vignette vignette;
    private TeleportationProvider teleportationProvider;

    protected override void Awake()
    {
        base.Awake();
        // Find the Volume component on the main camera to access vignette
        postProcessingVolume = Camera.main.GetComponent<Volume>();
        if (postProcessingVolume != null && postProcessingVolume.profile.TryGet<Vignette>(out var vignetteEffect))
        {
            vignette = vignetteEffect;
        }
        // Find the teleportation provider in the scene (attach it to an empty GameObject if needed)
        teleportationProvider = FindObjectOfType<TeleportationProvider>();
    }
    
 

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        // Ensure teleportation provider and target transform exist
        if (teleportationProvider != null && teleportAnchorTransform != null)
        {
            // Start the smooth teleport coroutine with the target position
            StartCoroutine(SmoothTeleportCoroutine(teleportAnchorTransform.position));
        }
    }

    private IEnumerator SmoothTeleportCoroutine(Vector3 targetPosition)
{
    // Ensure the XR Rig (or player root) is properly referenced
    GameObject xrRig = GameObject.Find("XR Rig"); // Replace with your XR Rig object if needed
    if (xrRig == null)
    {
        yield break; // Exit coroutine if XR Rig is not found
    }

    Transform playerTransform = xrRig.transform; // XR Rig transform (not just camera)
    Vector3 startPosition = playerTransform.position; // Get the starting position of the XR Rig
    float elapsed = 0f; // Track time elapsed in the transition

    // Ensure vignette is applied and setup
    if (vignette != null)
    {
        vignette.intensity.value = 1f; // Start with the maximum vignette intensity (full effect)
        vignette.smoothness.value = 0.4f; // Sharp vignette at the start
        vignette.color.value = Color.black; // A black vignette for a stronger effect
    }

    // Smoothly transition to the target position over time
    while (elapsed < 1f)
    {
        elapsed += Time.deltaTime * transitionSpeed; // Increase elapsed time based on the transition speed
        playerTransform.position = Vector3.Lerp(startPosition, targetPosition, elapsed); // Smoothly move the XR Rig

        // Keep vignette intensity at 1 during the teleport
        if (vignette != null)
        {
            vignette.intensity.value = 1f; // Keep vignette intensity at maximum during movement
        }

        yield return null; // Wait for the next frame
    }

    // After teleportation, start reducing the vignette intensity to 0
    if (vignette != null)
    {
        float vignetteElapsed = 0f;
        while (vignetteElapsed < 1f)
        {
            vignetteElapsed += Time.deltaTime * transitionSpeed;
            vignette.intensity.value = Mathf.Lerp(1f, 0f, vignetteElapsed); // Smoothly reduce vignette intensity
            vignette.smoothness.value = Mathf.Lerp(0.4f, 0.7f, vignetteElapsed); // Smoothly reduce sharpness of vignette
            yield return null; // Wait for the next frame
        }

        // Ensure vignette intensity is fully reset to 0
        vignette.intensity.value = 0f;
        vignette.smoothness.value = 0.7f; // Smooth vignette effect at the end
    }

    // Set the final position to ensure it ends at the target position (in case lerp doesn't perfectly reach)
    playerTransform.position = targetPosition;
}



}
