using UnityEngine;
using UnityEngine.UI;
using R3;

public class LightController : MonoBehaviour
{
    [SerializeField] Image directionalImage;

    [SerializeField] Image ambientImage;

    [SerializeField] Light directionalLight;

    private void Update()
    {
        directionalLight.color = directionalImage.color;

        RenderSettings.ambientLight = ambientImage.color;
    }

}
