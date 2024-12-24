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

        menuView.lightDirButton.onClick.AddListener(() => menu.SetAdvancedMode(AdvancedMenuMode.LightDir));
        menuView.envColorButton.onClick.AddListener(() => menu.SetAdvancedMode(AdvancedMenuMode.EnvColor));
        menuView.dirColorButton.onClick.AddListener(() => menu.SetAdvancedMode(AdvancedMenuMode.DirColor));
        menuView.charaShapeButton.onClick.AddListener(() => menu.SetAdvancedMode(AdvancedMenuMode.CharaShape));
        menuView.charaGazeButton.onClick.AddListener(() => menu.SetAdvancedMode(AdvancedMenuMode.CharaGaze));

        // Model -> View
        menu.mode.Subscribe(x => 
        {
            menuView.SetMode(x);
        }).AddTo(this);

        menu.isMenuOpen.Subscribe(x => 
        {
            menuView.SetOpen(x);
        }).AddTo(this);

        menu.advancedMode.Subscribe(x => 
        {
            menuView.SetAdvancedMode(x);
        }).AddTo(this);
    }

    private void OnDestroy()
    {
        menuView.leftSwipe.OnFinger.RemoveAllListeners();
        menuView.rightSwipe.OnFinger.RemoveAllListeners();
    }
}
