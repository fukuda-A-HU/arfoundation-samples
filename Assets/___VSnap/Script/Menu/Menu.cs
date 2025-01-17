using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    // public SerializableReactiveProperty<bool> isMenuOpen;
    public SerializableReactiveProperty<MenuMode> mode = new SerializableReactiveProperty<MenuMode>();
    public SerializableReactiveProperty<AdvancedMenuMode> advancedMode = new SerializableReactiveProperty<AdvancedMenuMode>();
    public ObservableList<AuthorView> authorButtons = new ObservableList<AuthorView>();
    public ObservableList<CharaPoseView> poseViews = new ObservableList<CharaPoseView>();
    public SerializableReactiveProperty<bool> isPoseCreditOpen = new SerializableReactiveProperty<bool>();

    private void Start()
    {
        mode.Publish();
    }
    private void SetMode(MenuMode mode)
    {
        this.mode.Value = mode;

        if (mode != MenuMode.Pose)
        {
            isPoseCreditOpen.Value = false;
        }
    }

    public void SetAdvancedMode(AdvancedMenuMode mode)
    {
        this.advancedMode.Value = mode;
    }

    public void SetModeLight(){
        if (mode.Value == MenuMode.PlaceChara)
        {
            SetMode(MenuMode.Basic);
        }
        else if (mode.Value == MenuMode.Basic){
            SetMode(MenuMode.Pose);
        }
        else if (mode.Value == MenuMode.Pose){
            SetMode(MenuMode.Advanced);
        }
        else if (mode.Value == MenuMode.Advanced){
            SetMode(MenuMode.GridView);
        }
    }

    public void SetModeLeft(){
        if (mode.Value == MenuMode.GridView){
            SetMode(MenuMode.Advanced);
        }
        else if (mode.Value == MenuMode.Advanced){
            SetMode(MenuMode.Pose);
        }
        else if (mode.Value == MenuMode.Pose){
            SetMode(MenuMode.Basic);
        }
        else if (mode.Value == MenuMode.Basic){
            SetMode(MenuMode.PlaceChara);
        }
    }

    private void OnDestroy()
    {
        mode.Dispose();
    }

}

public enum MenuMode{
    PlaceChara,
    Basic,
    Pose,
    Advanced,
    GridView
}

public enum AdvancedMenuMode{
    LightDir,
    EnvColor,
    DirColor,
    CharaShape,
    CharaGaze
}