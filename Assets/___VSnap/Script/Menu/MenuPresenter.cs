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

        menuView.lightDirButton.onClick.AddListener(() => menu.SetAdvancedMode(AdvancedMenuMode.LightDir));
        menuView.envColorButton.onClick.AddListener(() => menu.SetAdvancedMode(AdvancedMenuMode.EnvColor));
        menuView.dirColorButton.onClick.AddListener(() => menu.SetAdvancedMode(AdvancedMenuMode.DirColor));
        menuView.charaShapeButton.onClick.AddListener(() => menu.SetAdvancedMode(AdvancedMenuMode.CharaShape));
        menuView.charaGazeButton.onClick.AddListener(() => menu.SetAdvancedMode(AdvancedMenuMode.CharaGaze));

        // Model -> View
        menu.mode.Subscribe(x => 
        {
            menuView.SetMode(x);
            if (x == MenuMode.Advanced)
            {
                menuView.SetAdvancedMode(x, menu.advancedMode.Value);
            }
        }).AddTo(this);

        menu.advancedMode.Subscribe(x => 
        {
            menuView.SetAdvancedMode(menu.mode.Value, x);
        }).AddTo(this);

        menu.isPoseCreditOpen.Subscribe(x => 
        {
            menuView.SetPoseCredit(x, menu.mode.Value);
        }).AddTo(this);

        menuView.posesCreditButton.onClick.AddListener(() => 
        {
            menu.isPoseCreditOpen.Value = !menu.isPoseCreditOpen.Value;
        });
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
