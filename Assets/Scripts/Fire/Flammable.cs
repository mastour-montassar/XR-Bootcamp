using UnityEngine;

public class Flammable : MonoBehaviour
{
    [Tooltip("The particle effect for fire")]
    public ParticleSystem fireEffect;

    [Tooltip("Time (in seconds) for the fire to naturally extinguish")]
    public float extinguishTime = 5f;
    private bool isOnFire = false;

    public bool IsOnFire => isOnFire;

    private void Start()
    {
        fireEffect.Stop();
    }

    public void Ignite()
    {
        if (isOnFire) return;

        isOnFire = true;
        fireEffect.Play();

        // Start timer to extinguish fire automatically
        Invoke(nameof(Extinguish), extinguishTime);
    }

    public void Extinguish()
    {
        if (!isOnFire) return;

        isOnFire = false;
        fireEffect.Stop();
        CancelInvoke(nameof(Extinguish));
    }
}