using UnityEngine;
using UniVRM10;
using R3;

public class Chara : MonoBehaviour
{
    public SerializableReactiveProperty<GameObject> chara = new SerializableReactiveProperty<GameObject>();
    [SerializeField] private GameObject prevChara;
    public SkinnedMeshRenderer[] GetRenderers()
    {
        // 自分を含むこの全てに含まれるSkinnedMeshRendeererを取得
        if (chara.Value == null)
        {
            return null;
        }
        var renderers = chara.Value.GetComponentsInChildren<SkinnedMeshRenderer>();
        Debug.Log("renerer length " + renderers.Length);
        return renderers;
    }

    public void SetChara(GameObject chara)
    {
        if (chara == null)
        {
            return;
        }

        if (this.chara.Value != null)
        {
            Destroy(this.chara.Value);
            this.chara.Value = null;
        }
        this.chara.Value = chara;
    }

    private void Start()
    {
        chara.Subscribe(value =>
        {
            if (prevChara != null)
            {
                Destroy(prevChara);
                prevChara = null;
            }
            prevChara = value;
        }).AddTo(this);
    }
}
