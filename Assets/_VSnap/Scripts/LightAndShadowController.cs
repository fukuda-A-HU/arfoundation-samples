using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using R3;

public class LightAndShadowController : MonoBehaviour
{
    [SerializeField] Transform lightTransform;
    [SerializeField] Slider shadowSlider;

    [SerializeField] Slider heightSlider;

    [SerializeField] Slider rotationSlider;

    [SerializeField] Button reset;

    [SerializeField] Material shadowMaterial;

    public SerializableReactiveProperty<float> shadowAlpha = new SerializableReactiveProperty<float>();

    void Start(){

        heightSlider.onValueChanged.AddListener(ChangeRotation);

        rotationSlider.onValueChanged.AddListener(ChangeRotation);

        shadowSlider.onValueChanged.AddListener(ChangeShadow);

        reset.onClick.AddListener(Reset);

        lightTransform.rotation = Quaternion.Euler(new Vector3(heightSlider.value, rotationSlider.value, 0));
    }

    void ChangeRotation(float value){

        lightTransform.rotation = Quaternion.Euler(new Vector3(heightSlider.value, rotationSlider.value, 0));
    }

    void ChangeShadow(float value){
        
        var propertyName = shadowMaterial.shader.GetPropertyName(0);

        shadowMaterial.SetColor(propertyName, new Color(0, 0, 0, value));
    }

    void Reset(){

        heightSlider.value = 45;

        rotationSlider.value = 0;

        shadowSlider.value = 120;
    }

    void OnDestroy(){

        heightSlider.onValueChanged.RemoveListener(ChangeRotation);

        rotationSlider.onValueChanged.RemoveListener(ChangeRotation);

        shadowSlider.onValueChanged.RemoveListener(ChangeShadow);

        reset.onClick.RemoveListener(Reset);
    }
}
