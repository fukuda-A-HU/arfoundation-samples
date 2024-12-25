using Cysharp.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using R3;

public class MenuPresenter : MonoBehaviour
{
    [SerializeField] private MenuView menuView;
    [SerializeField] private Menu menu;
    [SerializeField] private CharaTransform charaTransform;

    private void Start()
    {
        // View -> Model
        menuView.leftSwipe.OnFinger.AddListener(x => menu.SetModeLeft());
        menuView.rightSwipe.OnFinger.AddListener(x => menu.SetModeLight());

        menuView.upSwipe.OnFinger.AddListener(x => menu.SetOpen(true));
        menuView.downSwipe.OnFinger.AddListener(x => menu.SetOpen(false));

        menuView.modeLeftButton.onClick.AddListener(() => menu.SetModeLeft());
        menuView.modeRightButton.onClick.AddListener(() => menu.SetModeLight());

        menuView.lightDirButton.onClick.AddListener(() => menu.SetAdvancedMode(AdvancedMenuMode.LightDir));
        menuView.envColorButton.onClick.AddListener(() => menu.SetAdvancedMode(AdvancedMenuMode.EnvColor));
        menuView.dirColorButton.onClick.AddListener(() => menu.SetAdvancedMode(AdvancedMenuMode.DirColor));
        menuView.charaShapeButton.onClick.AddListener(() => menu.SetAdvancedMode(AdvancedMenuMode.CharaShape));
        menuView.charaGazeButton.onClick.AddListener(() => menu.SetAdvancedMode(AdvancedMenuMode.CharaGaze));

        // Model -> View
        menu.mode.Subscribe(x => 
        {
            menuView.SetMode(x);

            if (x == MenuMode.PlaceChara)
            {
                charaTransform.isTranformable.Value = true;
            }
            else
            {
                charaTransform.isTranformable.Value = false;
            }
        }).AddTo(this);

        menu.isMenuOpen.Subscribe(x => 
        {
            menuView.SetOpen(x, menu.mode.Value);
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

        menuView.upSwipe.OnFinger.RemoveAllListeners();
        menuView.downSwipe.OnFinger.RemoveAllListeners();

        menuView.modeLeftButton.onClick.RemoveAllListeners();
        menuView.modeRightButton.onClick.RemoveAllListeners();
    }
}
