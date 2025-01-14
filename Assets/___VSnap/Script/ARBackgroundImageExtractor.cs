using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARBackgroundImageExtractor : MonoBehaviour
{
    private ARCameraBackground arCameraBackground;

    void Start()
    {
        arCameraBackground = FindObjectOfType<ARCameraBackground>();
        if (arCameraBackground == null)
        {
            Debug.LogError("ARCameraBackground component not found.");
        }
    }

    [ContextMenu("Extract Camera Image")]
    public Texture2D ExtractCameraImage()
    {
        if (arCameraBackground == null)
        {
            Debug.LogError("ARCameraBackground component is not set.");
            return null;
        }

        Material cameraMaterial = arCameraBackground.material;
        if (cameraMaterial == null)
        {
            Debug.LogError("Camera material is not set.");
            return null;
        }

        // カメラのテクスチャを取得
        RenderTexture renderTexture = cameraMaterial.mainTexture as RenderTexture;
        if (renderTexture == null)
        {
            Debug.LogError("Failed to get render texture from camera material.");
            return null;
        }

        // RenderTextureをTexture2Dに変換
        Texture2D cameraImage = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        cameraImage.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        cameraImage.Apply();
        RenderTexture.active = null;

        Debug.Log("Camera image extracted successfully. Size: " + cameraImage.width + "x" + cameraImage.height);

        // 画像を保存
        byte[] pngData = cameraImage.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/CameraImage.png", pngData);

        return cameraImage;
    }
}