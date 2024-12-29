using UnityEngine;
using R3;
using ObservableCollections;
using Cysharp.Threading.Tasks;
using System;


public class ShapeKeyPresenter : MonoBehaviour
{
    [SerializeField] private ShapeKey shapeKey;
    [SerializeField] private ShapeKeySliderView sliderView;
    private IDisposable disposable = null;

    private void Start()
    {
        // view -> model は今のうちに。 model -> viewはshapeKeyのIDが変更された時に
        sliderView.slider.onValueChanged.AddListener(value =>
        {
            shapeKey.SetBlendShapeWeight(value);
        });

        // model -> view
        shapeKey.selectedIDAndIndex.Subscribe(idAndIndex =>
        {
            if (disposable != null)
            {
                disposable.Dispose();
            }

            if (idAndIndex == null || idAndIndex.Length != 2) return;

            // shapeKey.shapeKeyValues[idAndIndex[0]][idAndIndex[2]]の値を購読して、sliderViewの値を変更する
            disposable = shapeKey.shapeKeyValues[idAndIndex[0]][idAndIndex[1]].Subscribe(value =>
            {
                sliderView.slider.value = value;
            }).AddTo(this);
            
            sliderView.SetName(shapeKey.GetRendererObjName(idAndIndex[0]), shapeKey.GetShapeKeyName(idAndIndex[0], idAndIndex[1]));

        }).AddTo(this);
    }

    private void OnDestroy()
    {
        sliderView.slider.onValueChanged.RemoveAllListeners();
    }


    public void OnCreateShapeKeyView(ShapeKeyView view, ShapeKey _shapeKey)
    {
        view.button.onClick.AddListener(() =>
        {
            _shapeKey.selectedIDAndIndex.Value = new int[] { view.rendererInstanceID, view.shapeKeyIndex };
        });

        _shapeKey.selectedIDAndIndex.Subscribe(idAndIndex =>
        {
            if (idAndIndex == null || idAndIndex.Length != 2) return;

            if (idAndIndex[0] == view.rendererInstanceID && idAndIndex[1] == view.shapeKeyIndex)
            {
                view.button.interactable = false;
            }
            else
            {
                view.button.interactable = true;
            }
        }).AddTo(this);

        _shapeKey.shapeKeyValues[view.rendererInstanceID][view.shapeKeyIndex].Subscribe(value =>
        {
            view.SetColor(value);
        }).AddTo(this);
    }

}
