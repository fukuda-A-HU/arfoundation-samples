using UnityEngine;
using UnityEngine.UI;

public class CharaTransformView : MonoBehaviour
{
    public Slider heightSlider;
    public Slider rotationSlider;
    public Slider scaleSlider;

    public void SetHeight(float value)
    {
        heightSlider.value = value;
    }

    public void SetRotation(float value)
    {
        rotationSlider.value = value;
    }

    public void SetScale(float value)
    {
        scaleSlider.value = value;
    }
}
