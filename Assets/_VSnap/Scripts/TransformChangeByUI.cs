using UnityEngine;
using UnityEngine.UI;
using R3;

public class TransformChangeByUI : MonoBehaviour
{
    [SerializeField]
    private Slider heightSlider;

    [SerializeField]
    private Slider rotateSlider;

    [SerializeField]
    private Button heightResetButton;

    [SerializeField]
    private Button rotateResetButton;

    public SerializableReactiveProperty<Vector3> offsetPosition = new SerializableReactiveProperty<Vector3>();

    public SerializableReactiveProperty<Vector3> offsetAngle = new SerializableReactiveProperty<Vector3>();

    public void heightChange()
    {
        var t = offsetPosition.Value;

        offsetPosition.Value = new Vector3(t.x, heightSlider.value, t.z);
    }

    public void rotateChange()
    {
        var angles = offsetAngle.Value;

        angles.y = rotateSlider.value;

        offsetAngle.Value = angles;
    }

    public void heightReset()
    {
        var t = offsetPosition.Value;

        offsetPosition.Value = new Vector3(t.x, 0, t.z);

        heightSlider.value = 0;
    }

    public void rotationReset()
    {
        var angles = offsetAngle.Value;

        angles.y =  0;

        offsetAngle.Value = angles;

        rotateSlider.value = 0;
    }
}
