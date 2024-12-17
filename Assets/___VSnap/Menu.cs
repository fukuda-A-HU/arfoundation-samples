using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public SerializableReactiveProperty<bool> isMenuOpen;
    public SerializableReactiveProperty<MenuMode> mode;
    public SerializableReactiveProperty<string> authorName;
    public SerializableReactiveProperty<AdvancedMenuMode> advancedMode;
    public ObservableList<AuthorView> authorButtons = new ObservableList<AuthorView>();
    public ObservableList<PoseView> poseViews = new ObservableList<PoseView>();

    public void SetOpen(bool isOpen)
    {
        this.isMenuOpen.Value = isOpen;
    }

    private void SetMode(MenuMode mode)
    {
        this.mode.Value = mode;
    }

    public void SetAdvancedMode(AdvancedMenuMode mode)
    {
        this.advancedMode.Value = mode;
    }

    public void SetAuthor(string authorName)
    {
        this.authorName.Value = authorName;
    }

    public void SetModeLight(){
        if (mode.Value == MenuMode.Basic){
            SetMode(MenuMode.Pose);
        }
        else if (mode.Value == MenuMode.Pose){
            SetMode(MenuMode.Advanced);
        }
    }

    public void SetModeLeft(){
        if (mode.Value == MenuMode.Advanced){
            SetMode(MenuMode.Pose);
        }
        else if (mode.Value == MenuMode.Pose){
            SetMode(MenuMode.Basic);
        }
    }

    private void OnDestroy()
    {
        mode.Dispose();
    }

}

public enum MenuMode{
    Basic,
    Pose,
    Advanced
}

public enum AdvancedMenuMode{
    LightDir,
    EnvColor,
    DirColor,
    CharaShape,
    CharaGaze
}

public enum LightDirMode{
    Height,
    Rot
}

public enum EnvColorMode{
    Hue,
    Sat,
    Lum
}

public enum DirColorMode{
    Hue,
    Sat,
    Lum
}

public enum GazeMode{
    Eye,
    Head,
    Body
}