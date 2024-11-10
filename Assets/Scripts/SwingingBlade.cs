using UnityEngine;

public class SwingingBlade : MonoBehaviour
{
    public float swingSpeed = 2f; // Speed of the swing
    public float swingAngle = 45f; // Maximum angle of the swing

    private Quaternion startRotation;

    void Start()
    {
        // Store the starting rotation
        startRotation = transform.rotation;
    }

    void Update()
    {
        // Calculate the angle for the current time using PingPong
        float angle = Mathf.PingPong(Time.time * swingSpeed, swingAngle * 2) - swingAngle;
        
        // Set the rotation
        transform.rotation = startRotation * Quaternion.Euler(angle, 0,0 );
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Handle collision with the player (e.g., reduce health or respawn)
            Debug.Log("Player hit by the blade!");
        }
    }

}