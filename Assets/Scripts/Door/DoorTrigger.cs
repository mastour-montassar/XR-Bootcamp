using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public static bool intTriger = false; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<DoorInteractor>())
        {
            intTriger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<DoorInteractor>())
        {
            intTriger = false; 
        }
    }
}
