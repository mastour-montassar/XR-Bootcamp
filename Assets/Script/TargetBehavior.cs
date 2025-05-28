using UnityEngine;

public class TargetBehavior : MonoBehaviour
{
    private Renderer targetRenderer;
    public Material inactiveMaterial; // Transparent material
    public Material activeMaterial;   // Opaque material (or glowing effect)
    public ParticleSystem hitEffect;  // Effect to play on hit
    public int scoreValue = 10;
    private ScoreManager scoreManager;
    public bool isActive = false;

    void Start()
    {
        // Get the Renderer component
        targetRenderer = GetComponent<Renderer>();
        SetInactive();
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    public void LightUp()
    {
        if (!isActive)
        {
            // Set the material to active
            targetRenderer.material = activeMaterial;
            isActive = true;

            // Schedule deactivation after a random time (for example, 2 seconds)
            Invoke("SetInactive", 2f);
        }
    }

    public void SetInactive()
    {
        targetRenderer.material = inactiveMaterial;
        isActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActive && other.gameObject.layer == LayerMask.NameToLayer("Controller"))
        {
            // Handle a hit
            Debug.Log("Target Hit!");
            isActive = false;
            scoreManager.AddScore(scoreValue);
            SetInactive();

            // Play hit effect if available
            if (hitEffect != null)
            {
                // Detach the particle system to play it at the hit location
                ParticleSystem effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                effect.Play();
                Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax); // Destroy after it finishes
            }
        }
    }
}