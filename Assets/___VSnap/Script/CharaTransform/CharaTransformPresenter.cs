using UnityEngine;
using R3;
using Lean.Touch;
using Cysharp.Threading.Tasks;

public class CharaTransformPresenter : MonoBehaviour
{
    [SerializeField] private CharaTransform charaTransform;
    [SerializeField] private CharaTransformView view;
    [SerializeField] private Menu menu;

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

        LeanTouch.OnFingerDown += (finger) =>
        {
            charaTransform.RecordPrevPosition();
        };

        LeanTouch.OnFingerUp += (finger) =>
        {
            // 3フレーム待ってから、menu modeがcharaの時だけ、キャラの位置を決定する
            UniTask.DelayFrame(3).ContinueWith(() =>
            {
                if (menu.mode.Value != MenuMode.PlaceChara)
                {
                    charaTransform.SetPrevPosition();
                }
            });
        };
    }

    private void OnDestroy()
    {
        view.heightSlider.onValueChanged.RemoveAllListeners();

        view.rotationSlider.onValueChanged.RemoveAllListeners();

        view.scaleSlider.onValueChanged.RemoveAllListeners();
    }
}
