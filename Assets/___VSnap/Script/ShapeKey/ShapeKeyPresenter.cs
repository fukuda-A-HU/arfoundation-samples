using UnityEngine;
using R3;
using ObservableCollections;
using Cysharp.Threading.Tasks;


public class ShapeKeyPresenter : MonoBehaviour
{
    [SerializeField] private ShapeKey shapeKey;

    public void OnCreateShapeKeyView(ShapeKeyView view)
    {
        view.currentValue.Subscribe(value => 
        {
            shapeKey.SetBlendShapeWeight(view.rendererInstanceID, view.shapeKeyIndex, value);
        }).AddTo(this);
    }

}
