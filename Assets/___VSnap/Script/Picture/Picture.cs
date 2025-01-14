using UnityEngine;
using System;
using System.IO;
using R3;

public class Picture : MonoBehaviour
{
    [SerializeField] private Camera _captureCamera;
    public SerializableReactiveProperty<bool> isPreviewing = new SerializableReactiveProperty<bool>();
    public SerializableReactiveProperty<Texture2D> pictureTexture = new SerializableReactiveProperty<Texture2D>();

    public void TakePicture()
    {
        // スクショ用の、ARカメラ描画結果を格納するRenderTextureを用意する

        // int height = (int)(imageAspect * Screen.width);
        int height = Screen.height;
        RenderTexture rt = RenderTexture.GetTemporary(Screen.width, height, 24, RenderTextureFormat.ARGB32);

        Debug.Log("initial texture" + height + " " + Screen.width);

        // 用意したRenderTextureに書き込む
        RenderTexture prevTarget = _captureCamera.targetTexture;
        _captureCamera.targetTexture = rt;
        _captureCamera.Render();
        _captureCamera.targetTexture = prevTarget;

        Debug.Log("Camera Texture " +  _captureCamera.pixelHeight + " " + _captureCamera.pixelWidth);

        // RenderTextureのままでは保存できないので、Textureに変換する
        RenderTexture prevActive = RenderTexture.active;
        RenderTexture.active = rt;
        var screenShot = new Texture2D(_captureCamera.pixelWidth, height, TextureFormat.ARGB32, false);

        Debug.Log("Screen Texture  " + height + " " + screenShot.width);
        // Debug.Log("CPU Texture  " + cpuImageTexture.Value.height + " " + cpuImageTexture.Value.width);
        
        screenShot.ReadPixels(new Rect(0, 0, screenShot.width, height), 0, 0, false);
        screenShot.Apply();

        // 保存処理
        pictureTexture.Value = screenShot;
        isPreviewing.Value = true;
        try
        {
            string timestamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");

            NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(screenShot, "ShreenshotSample", $"screenshot_{timestamp}.jpg", (success, path) =>
            {
                // 保存終了時のコールバック
            });
        }
        catch (IOException e)
        {
            // 保存時エラーが出た時の処理を記述
            Debug.Log("Error: " + e);
        }
        finally
        {
            // 最後にARカメラの描画先をスクリーンに戻す
            RenderTexture.ReleaseTemporary(rt);
            RenderTexture.active = prevActive;
        }
    }

    public void SetPreviewImage(bool isOn)
    {
        isPreviewing.Value = isOn;
    }

}
