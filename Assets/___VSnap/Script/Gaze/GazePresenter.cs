using UnityEngine;
using R3;

public class GazePresenter : MonoBehaviour
{
    [SerializeField] private GazeView view;
    [SerializeField] private Gaze gaze;

    private void Start()
    {
        gaze.eyeValue.Subscribe(value =>
        {
            view.SetEyeValue(value);
        }).AddTo(this);

        gaze.headValue.Subscribe(value =>
        {
            view.SetHeadValue(value);
        }).AddTo(this);

        gaze.bodyValue.Subscribe(value =>
        {
            view.SetBodyValue(value);
        }).AddTo(this);

        view.eyeSlider.onValueChanged.AddListener(value =>
        {
            gaze.SetEyeWeight(value);
        });

        view.headSlider.onValueChanged.AddListener(value =>
        {
            gaze.SetHeadWeight(value);
        });

        view.bodySlider.onValueChanged.AddListener(value =>
        {
            gaze.SetBodyWeight(value);
        });

    }

    private void OnDestroy()
    {
        view.eyeSlider.onValueChanged.RemoveAllListeners();
        view.headSlider.onValueChanged.RemoveAllListeners();
        view.bodySlider.onValueChanged.RemoveAllListeners();
    }
}
