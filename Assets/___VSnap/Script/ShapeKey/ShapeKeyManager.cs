using ObservableCollections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using R3;

public class ShapeKeyManager : MonoBehaviour
{
    public ObservableList<ShapeKeyView> shapeKeyViews = new ObservableList<ShapeKeyView>();
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private RectTransform buttonParent;

    [SerializeField] private ShapeKey shapeKeyComponent;

    private void Start()
    {
        // 追加された時の処理
        shapeKeyComponent.shapeKeyValues.ObserveAdd().Subscribe(async shapeKeyValue =>
        {
            var rendererName = shapeKeyComponent.GetRendererObjName(shapeKeyValue.Value.Key);
            foreach (var shapeKey in shapeKeyValue.Value.Value)
            {
                var shapeKeyName = shapeKeyComponent.GetShapeKeyName(shapeKeyValue.Value.Key, shapeKey.Key);
                var viewPrefab = await InstantiateAsync(buttonPrefab, buttonParent);
                var view = viewPrefab[0].GetComponent<ShapeKeyView>();
                view.SetName(rendererName, shapeKeyName);
                view.rendererInstanceID = shapeKeyValue.Value.Key;
                view.shapeKeyIndex = shapeKey.Key;
                shapeKeyViews.Add(view);
            }
        }).AddTo(this);

        // 削除された時の処理
        shapeKeyComponent.shapeKeyValues.ObserveRemove().Subscribe(shapeKeyValue =>
        {
            foreach (var view in shapeKeyViews)
            {
                if (view.rendererInstanceID == shapeKeyValue.Value.Key)
                {
                    shapeKeyViews.Remove(view);
                    Destroy(view.gameObject);
                }
            }
        }).AddTo(this);
    }

}
