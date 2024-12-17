using Cysharp.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using R3;

public class MenuPresenter : MonoBehaviour
{
    [SerializeField] private MenuView menuView;
    [SerializeField] private Menu menu;

    private void Start()
    {
        // View -> Model
        menuView.leftSwipe.OnFinger.AddListener(x => menu.SetModeLeft());
        menuView.rightSwipe.OnFinger.AddListener(x => menu.SetModeLight());
        
        menuView.upSwipe.OnFinger.AddListener(x => menu.SetOpen(true));
        menuView.downSwipe.OnFinger.AddListener(x => menu.SetOpen(false));

        // Model -> View
        menu.mode.Subscribe(x => 
        {
            menuView.SetMode(x);
        }).AddTo(this);

        menu.isMenuOpen.Subscribe(x => 
        {
            menuView.SetOpen(x);
        }).AddTo(this);


    }

    private void OnDestroy()
    {
        menuView.leftSwipe.OnFinger.RemoveAllListeners();
        menuView.rightSwipe.OnFinger.RemoveAllListeners();
    }
}
