using UnityEngine;

public class WaterSource : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Flammable"))
        {
            Debug.Log("Water hit flammable object");
            Flammable flammableObject = other.GetComponent<Flammable>();
            if (flammableObject != null && flammableObject.IsOnFire)
            {
                flammableObject.Extinguish();

                // Find the specific child object representing fire
                Transform fireEffect = flammableObject.transform.Find("VFX_Fire_Floor_01_Simple (1)"); 
                if (fireEffect != null)
                {
                    fireEffect.gameObject.SetActive(false); 
                }
            }
        }
    }
}