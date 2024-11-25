using UnityEngine;
using UnityEngine.UI;

public class LightColorController : MonoBehaviour
{
    public Light targetLight;
    public Slider redSlider, greenSlider, blueSlider;

    void Update()
    {
        Color newColor = new Color(redSlider.value, greenSlider.value, blueSlider.value);
        targetLight.color = newColor;
    }
}