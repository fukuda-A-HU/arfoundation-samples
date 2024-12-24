using UnityEngine;
using R3;
using ObservableCollections;

public class ShapeKeyDispacher : MonoBehaviour
{
    [SerializeField] private ShapeKeyManager manager;
    [SerializeField] private ShapeKeyPresenter presenter;
    [SerializeField] private ShapeKey shapeKey;

    private void Start()
    {
        // 今リストにあるやつをDispatch
        foreach (var v in manager.shapeKeyViews)
        {
            DispatchShapeKey(v);
        }

        manager.shapeKeyViews.ObserveAdd().Subscribe(p => DispatchShapeKey(p.Value));
    }

    private void DispatchShapeKey(ShapeKeyView view)
    {
       presenter.OnCreateShapeKeyView(view);
    }

}
