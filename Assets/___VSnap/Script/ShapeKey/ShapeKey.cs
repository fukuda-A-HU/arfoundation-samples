using UnityEngine;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;


public class ShapeKey : MonoBehaviour
{
    //public ObservableList<SkinnedMeshRenderer> renderers = new ObservableList<SkinnedMeshRenderer>();
    public ObservableDictionary<int, SkinnedMeshRenderer> renderers = new ObservableDictionary<int, SkinnedMeshRenderer>();

    // 変更時に通知するため、ObservableDictionaryによってWeightを管理
    public ObservableDictionary<int, ObservableDictionary<int, float>> shapeKeyValues = new ObservableDictionary<int, ObservableDictionary<int, float>>();


    public void AddRenderer(SkinnedMeshRenderer renderer)
    {
        renderers.Add(renderer.GetInstanceID(), renderer);
    }

    public void SetBlendShapeWeight(int rendererInstanceID, int index, float value)
    {
        renderers[rendererInstanceID].SetBlendShapeWeight(index, value);
    }

    public string GetRendererObjName(int rendererInstanceID)
    {
        return renderers[rendererInstanceID].gameObject.name;
    }

    public string GetShapeKeyName(int rendererInstanceID, int index)
    {
        return renderers[rendererInstanceID].sharedMesh.GetBlendShapeName(index);
    }

    private void Start()
    {
        // Rendererが追加された時の処理
        renderers.ObserveAdd().Subscribe(renderer =>
        {
            var shapeKeyValue = new ObservableDictionary<int, float>();
            for (int i = 0; i < renderer.Value.Value.sharedMesh.blendShapeCount; i++)
            {
                shapeKeyValue.Add(i, renderer.Value.Value.GetBlendShapeWeight(i));
            }

            shapeKeyValues.Add(renderer.Value.Key, shapeKeyValue);
        }).AddTo(this);

        // Rendererが削除された時の処理
        renderers.ObserveRemove().Subscribe(renderer =>
        {
            shapeKeyValues.Remove(renderer.Value.Key);
        }).AddTo(this);
    }

    [ContextMenu("Check")]
    private void Check()
    {
        // Renderer, ShapeKeyValuesの内容を表示
        foreach (var renderer in renderers)
        {
            Debug.Log("Renderer : " + renderer.Value.gameObject.name);
            foreach (var shapeKeyValue in shapeKeyValues[renderer.Value.GetInstanceID()])
            {
                Debug.Log("ShapeKey : " + shapeKeyValue.Key + " : " + shapeKeyValue.Value);
            }
        }
    }

}