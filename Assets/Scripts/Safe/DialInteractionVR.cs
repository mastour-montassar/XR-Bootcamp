using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI; // For Unity UI Text
using TMPro; // For TextMeshPro (optional)

public class DialInteractionVR : MonoBehaviour
{
    public int totalNumbers = 10;        // Number of values on the dial (e.g., 0-9)
    public float degreesPerNumber = 36f;  // Degrees required to move between numbers
    public int currentValue = 0;         // Current number on the dial

    private float totalRotation = 0f;    // Tracks cumulative rotation
    private XRBaseInteractor interactor; // Reference to interacting controller/hand
    private float previousAngle;

    public bool isLocked = true;          // Tracks whether the dial is locked
    public int unlockValue = 1;          // The value that unlocks the safe

    public GameObject door;              // Reference to the door object (for Task 6)

    // UI Elements
    public Text  currentValueText;        // Unity UI Text for the current value
    public Text  unlockValueText;         // Unity UI Text for the unlock value
    // OR, if using TextMeshPro:
    // public TMP_Text currentValueText;
    // public TMP_Text unlockValueText;

    void Start()
    {
        totalRotation = 0f;
        currentValue = 0;

        // Initialize text fields
        if (currentValueText != null)
            currentValueText.text = $"Current Value: {currentValue}";
        if (unlockValueText != null)
            unlockValueText.text = $"Unlock Value: {unlockValue}";
    }

    public void OnGrabDial(SelectEnterEventArgs args)
    {
        // If locked, allow rotation for counting, but do not unlock
        interactor = args.interactor as XRBaseInteractor;

        if (interactor != null)
        {
            // Calculate the initial angle
            previousAngle = CalculateInteractorAngle();
        }
    }

    public void OnReleaseDial(SelectExitEventArgs args)
    {
        // Clear the interactor when released
        interactor = null;
    }

    void Update()
    {
        // Always update the dial rotation and value
        if (interactor != null)
        {
            // Get the current angle based on the interactor position
            float currentAngle = CalculateInteractorAngle();

            // Calculate the angle delta (how much the dial has turned since the last frame)
            float angleDelta = Mathf.DeltaAngle(previousAngle, currentAngle);

            // Update the total rotation
            totalRotation += angleDelta;

            // Normalize the total rotation to stay within a 0-360 degree range
            float normalizedRotation = totalRotation % 360f;

            // Ensure positive values for normalized rotation
            if (normalizedRotation < 0)
                normalizedRotation += 360f;

            // Calculate the current number based on the normalized rotation
            currentValue = Mathf.FloorToInt(normalizedRotation / degreesPerNumber) % totalNumbers;

            // Update the UI with the current value
            if (currentValueText != null)
                currentValueText.text = $"Current Value: {currentValue}";

            // Update the dial's rotation (you can lock this to any axis as needed)
            transform.localRotation = Quaternion.Euler(0, normalizedRotation, 0);

            // Save the current angle for the next frame
            previousAngle = currentAngle;
        }

        // Check unlock condition only if locked
        if (isLocked)
        {
            CheckUnlockCondition();
        }
    }

    private float CalculateInteractorAngle()
    {
        if (interactor == null) return 0f;

        // Get the direction from the dial to the interactor
        Vector3 direction = interactor.transform.position - transform.position;

        // Project onto the XZ plane (for Y-axis rotation)
        direction.y = 0;

        // Calculate the angle in the horizontal plane
        return Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
    }

    private void CheckUnlockCondition()
    {
        // Only check unlocking when the value is correct and the safe is locked
        if (currentValue == unlockValue)
        {
            UnlockSafe();
        }
    }

    private void UnlockSafe()
    {
        if (!isLocked) return;

        isLocked = false;
        Debug.Log("Safe Unlocked!");

        // Update UI to show unlocked state
        if (currentValueText != null)
            currentValueText.text = "Safe Unlocked!";
        
        gameObject.SetActive(false);
        
        if (door != null)
        {
            door.GetComponent<Animator>()?.SetTrigger("OnTrigger");
        }

        // Disable this script and any interaction logic
        this.enabled = false;
    }

    public void LockSafe()
    {
        isLocked = true;
        Debug.Log("Safe Locked!");

        // Reset the text UI if relocking
        if (currentValueText != null)
            currentValueText.text = $"Current Value: {currentValue}";
        if (unlockValueText != null)
            unlockValueText.text = $"Unlock Value: {unlockValue}";
    }
}
