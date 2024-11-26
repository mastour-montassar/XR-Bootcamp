using UnityEngine;

public class ControlVolume : MonoBehaviour
{
    private bool isHandInside = false;

    private void OnTriggerEnter(Collider other)
    {
            isHandInside = true;
            Debug.Log("Hand entered control volume");

    }

    private void OnTriggerExit(Collider other)
    {

            isHandInside = false;
            Debug.Log("Hand exited control volume");
    }

    public bool IsHandInside()
    {
        return isHandInside;
    }
}