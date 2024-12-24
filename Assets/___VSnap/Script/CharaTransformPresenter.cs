using UnityEngine;
using R3;

using Cysharp.Threading.Tasks;

public class CharaTransformPresenter : MonoBehaviour
{
    [SerializeField] private CharaTransform charaTransform;
    [SerializeField] private CharaTransformView view;

    private void Start()
    {
        view.heightSlider.onValueChanged.AddListener(value =>
        {
            charaTransform.SetHeight(value);
        });

        view.rotationSlider.onValueChanged.AddListener(value =>
        {
            charaTransform.SetRotation(value);
        });

        view.scaleSlider.onValueChanged.AddListener(value =>
        {
            charaTransform.SetScale(value);
        });

        charaTransform.height.Subscribe(value =>
        {
            view.SetHeight(value);
        }).AddTo(this);

        charaTransform.rotation.Subscribe(value =>
        {
            view.SetRotation(value);
        }).AddTo(this);

        charaTransform.scale.Subscribe(value =>
        {
            view.SetScale(value);
        }).AddTo(this);


    }

    private void OnDestroy()
    {
        view.heightSlider.onValueChanged.RemoveAllListeners();

        view.rotationSlider.onValueChanged.RemoveAllListeners();

        view.scaleSlider.onValueChanged.RemoveAllListeners();
    }
}
