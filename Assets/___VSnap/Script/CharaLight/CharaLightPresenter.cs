using UnityEngine;
using R3;

public class CharaLightPresenter : MonoBehaviour
{
    [SerializeField] private CharaLight charaLight;
    [SerializeField] private CharaLightView view;

    private void Start()
    {
        // model -> View
        charaLight.dirColor.Subscribe(x =>
        {
            view.SetDirColor(x);
        }).AddTo(this);

        charaLight.envColor.Subscribe(x =>
        {
            view.SetEnvColor(x);
        }).AddTo(this);

        charaLight.height.Subscribe(x =>
        {
            view.SetHeight(x);
        }).AddTo(this);

        charaLight.rotation.Subscribe(x =>
        {
            view.SetRotation(x);
        }).AddTo(this);

        // View -> Model
        view.heightSlider.onValueChanged.AddListener(x =>
        {
            charaLight.SetHeight(x);
        });

        view.rotationSlider.onValueChanged.AddListener(x =>
        {
            charaLight.SetRotation(x);
        });

        view.shadowIntensitySlider.onValueChanged.AddListener(x =>
        {
            charaLight.SetShadowIntensity(x);
        });
        
    }
}
