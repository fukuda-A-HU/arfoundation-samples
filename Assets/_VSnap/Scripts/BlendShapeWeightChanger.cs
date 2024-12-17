using UnityEngine;
using UnityEngine.UI;

public class BlendShapeWeightChanger : MonoBehaviour
{
    public SkinnedMeshRenderer renderer;

    public int blendShapeIndex = -1;

    [SerializeField]
    private Slider slider;

    public float MaxValue
    {
        set { slider.maxValue = value; }
    }

    public float MinValue
    {
        set { slider.minValue = value; }
    }

    public float currentValue
    {
        set { slider.value = value; }
    }

    public void Change()
    {
        if (renderer == null || blendShapeIndex == -1)
        {
            Debug.Log("Index or renderer not set!");
        }
        else
        {
            renderer.SetBlendShapeWeight(blendShapeIndex, slider.value);
        }
    }
}
