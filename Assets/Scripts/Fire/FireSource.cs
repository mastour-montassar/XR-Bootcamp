using UnityEngine;

public class FireSource : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Flammable"))
        {
            Debug.Log("Fire");
            Flammable flammableObject = other.GetComponent<Flammable>();
            if (flammableObject != null && !flammableObject.IsOnFire)
            {
                flammableObject.Ignite();

                // Find the specific child object representing fire
                Transform fireEffect = flammableObject.transform.Find("VFX_Fire_Floor_01_Simple (1)"); 
                if (fireEffect != null)
                {
                    fireEffect.gameObject.SetActive(true); // Activate the fire effect
                }
            }
        }
    }
}
