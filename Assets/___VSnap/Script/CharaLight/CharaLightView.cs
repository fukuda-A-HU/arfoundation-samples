using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharaLightView : MonoBehaviour
{
    public Slider heightSlider;
    public Slider rotationSlider;
    public Slider shadowIntensitySlider;

    public Image dirColorImage;
    public Image dirColorIcon;
    public TextMeshProUGUI dirColorText;
    public Image envColorImage;
    public Image envColorIcon;
    public TextMeshProUGUI envColorText;

    public void SetHeight(float value)
    {
        heightSlider.value = value;
    }

    public void SetRotation(float value)
    {
        rotationSlider.value = value;
    }

    public void SetShadowIntensity(float value)
    {
        shadowIntensitySlider.value = value;
    }

    public void SetDirColor(Color color)
    {
        color.a = 1;
        dirColorImage.color = color;

        dirColorIcon.color = CalculateTextColor(color);
        dirColorText.color = CalculateTextColor(color);
    }

    public void SetEnvColor(Color color)
    {
        color.a = 1;
        envColorImage.color = color;

        envColorIcon.color = CalculateTextColor(color);
        envColorText.color = CalculateTextColor(color);
    }

    private Color CalculateTextColor(Color bgColor)
    {
        // 輝度を計算 (Y = 0.299*R + 0.587*G + 0.114*B)
        float luminance = (0.299f * bgColor.r + 0.587f * bgColor.g + 0.114f * bgColor.b);

        // 輝度が0.5以上なら文字色を黒 (Color.black)、それ以外は白 (Color.white)
        return luminance > 0.5f ? Color.black : Color.white;
    }
}
