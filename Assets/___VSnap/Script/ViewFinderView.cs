using UnityEngine;
using UnityEngine.UI;

public class ViewFinderView : MonoBehaviour
{
    public RawImage image;

    public void SetTexture(Texture2D texture)
    {
        image.texture = texture;

        // 画像の大きさを調整
        float aspect = (float)texture.width / texture.height;
        float width = Screen.width;
        float height = width / aspect;
    }
}
