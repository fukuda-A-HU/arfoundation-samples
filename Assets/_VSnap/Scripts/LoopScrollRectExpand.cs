using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

[RequireComponent(typeof(LoopScrollRect))]
[DisallowMultipleComponent]
public sealed class LoopScrollRectExpand : MonoBehaviour, LoopScrollPrefabSource, LoopScrollDataSource
{
    [SerializeField] private GameObject _prefab;

    public int totalCount = -1;

    // データ数
    public int dataCount = 100;

    private ObjectPool<GameObject> _pool;

    private void Start()
    {
        _pool = new ObjectPool<GameObject>(
            () => Instantiate(_prefab),
            o => o.SetActive(true),
            o =>
            {
                o.transform.SetParent(transform);
                o.SetActive(false);
            });

        var scrollRect = GetComponent<LoopScrollRect>();
        scrollRect.prefabSource = this;
        scrollRect.dataSource = this;
        scrollRect.totalCount = totalCount;
        scrollRect.RefillCells();
    }

    void LoopScrollDataSource.ProvideData(Transform trans, int index)
    {
        // データのインデックスを求める
        var dataIndex = (int)Mathf.Repeat(index, dataCount);
        trans.GetChild(0).GetComponent<TextMeshProUGUI>().text = dataIndex.ToString();
    }

    GameObject LoopScrollPrefabSource.GetObject(int index)
    {
        return _pool.Get();
    }

    void LoopScrollPrefabSource.ReturnObject(Transform trans)
    {
        _pool.Release(trans.gameObject);
    }
}
