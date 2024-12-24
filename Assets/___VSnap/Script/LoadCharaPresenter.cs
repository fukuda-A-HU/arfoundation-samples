using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;

public class LoadCharaPresenter : MonoBehaviour
{
    [SerializeField] LoadChara loadChara;
    [SerializeField] Chara chara;
    [SerializeField] MenuView menuView;
    void Start()
    {
        loadChara.chara.Subscribe(value => 
        {
            Debug.Log("LoadCharaPresenter: chara.Value = " + value.GetInstanceID());
            chara.SetChara(value);
        }).AddTo(this);

        menuView.charaChangeButton.onClick.AddListener(() =>
        {
            loadChara.Load();
        });
    }

    void OnDestroy()
    {
        menuView.charaChangeButton.onClick.RemoveAllListeners();
    }
}
