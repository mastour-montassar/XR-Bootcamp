using UnityEngine;
using UnityEngine.UI;

public class LightController : MonoBehaviour
{
    public Light targetLight;
    public Button lightButton;
    public Text buttonText;  // Text component on the button

    private bool isLightOn = true;

    void Start()
    {
        lightButton.onClick.AddListener(ToggleLight);
        UpdateButtonText();  // Update button text based on the initial state
    }

    void ToggleLight()
    {
        isLightOn = !isLightOn;
        targetLight.enabled = isLightOn;
        UpdateButtonText();
        Debug.Log("Light Toggled: " + (isLightOn ? "On" : "Off"));
    }

    void UpdateButtonText()
    {
        buttonText.text = isLightOn ? "Turn Off Light" : "Turn On Light";
    }
}