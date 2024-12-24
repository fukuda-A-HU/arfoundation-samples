using UnityEngine;
using R3;
using ObservableCollections;
using Cysharp.Threading.Tasks;

public class CharaLight : MonoBehaviour
{
    [SerializeField] private ColorPicker ColorPickerRef;
    [SerializeField] private Menu menu;
    [SerializeField] private Light dirLight;
    [SerializeField] private Material shadowMaterial;

    public SerializableReactiveProperty<Color> dirColor;
    public SerializableReactiveProperty<Color> envColor;
    public SerializableReactiveProperty<float> height;
    public SerializableReactiveProperty<float> rotation;
    public SerializableReactiveProperty<float> shadowAlpha;

    void Start()
    {
        menu.advancedMode.Subscribe(x =>
        {
            if (x == AdvancedMenuMode.DirColor)
            {
                ColorPickerRef.Show(true);
                ColorPickerRef.UpdateSelectedColor(dirColor.Value);
            }
            else if (x == AdvancedMenuMode.EnvColor)
            {
                ColorPickerRef.Show(true);
                ColorPickerRef.UpdateSelectedColor(envColor.Value);
            }
            else
            {
                ColorPickerRef.Show(false);
            }
        }).AddTo(this);

        dirColor.Subscribe(x =>
        {
            dirLight.color = x;
        }).AddTo(this);

        envColor.Subscribe(x =>
        {
            RenderSettings.ambientLight = x;
        }).AddTo(this);

        height.Subscribe(x =>
        {
            dirLight.transform.rotation = Quaternion.Euler(x, rotation.Value, 0);
        }).AddTo(this);

        rotation.Subscribe(x =>
        {
            dirLight.transform.rotation = Quaternion.Euler(height.Value, x, 0);
        }).AddTo(this);

        shadowAlpha.Subscribe(x =>
        {
            var propertyName = shadowMaterial.shader.GetPropertyName(0);

            shadowMaterial.SetColor(propertyName, new Color(0, 0, 0, x));
        }).AddTo(this);

        ColorPickerRef.OnColorValueChanged += ChangeColorOfLight;
    }

    private void ChangeColorOfLight(Color newColor)
    {
        if (menu.advancedMode.Value == AdvancedMenuMode.DirColor)
        {
            SetDirColor(newColor);
        }
        else if (menu.advancedMode.Value == AdvancedMenuMode.EnvColor)
        {
            SetEnvColor(newColor);
        }
    }

    public void SetDirColor(Color color)
    {
        dirColor.Value = color;
    }

    public void SetEnvColor(Color color)
    {
        envColor.Value = color;
    }

    public void SetHeight(float height)
    {
        if (height < 0)
        {
            height = 0;
        }
        else if (height > 90)
        {
            height = 90;
        }
        this.height.Value = height;
    }

    public void SetRotation(float rotation)
    {
        // 360で割った余りを取ることで0~360の範囲に収める
        this.rotation.Value = rotation % 360;
    }

    public void SetShadowIntensity(float alpha)
    {
        shadowAlpha.Value = alpha;
    }
}
