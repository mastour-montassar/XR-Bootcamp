using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRSelectHandler : MonoBehaviour
{
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        if (rb != null)
        {
            rb.isKinematic = true; // Disable physics when selected
        }
    }

    public void OnSelectExit(SelectExitEventArgs args)
    {
        if (rb != null)
        {
            rb.isKinematic = false; // Re-enable physics when deselected
        }
    }
}
