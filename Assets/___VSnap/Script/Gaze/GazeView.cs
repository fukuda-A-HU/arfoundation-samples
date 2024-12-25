using UnityEngine;
using UnityEngine.UI;
using R3;

public class GazeView : MonoBehaviour
{
    public Slider eyeSlider;
    public Slider headSlider; 
    public Slider bodySlider;

    public void SetEyeValue(float value)
    {
        eyeSlider.value = value;
    }

    public void SetHeadValue(float value)
    {
        headSlider.value = value;
    }

    public void SetBodyValue(float value)
    {
        bodySlider.value = value;
    }
}
