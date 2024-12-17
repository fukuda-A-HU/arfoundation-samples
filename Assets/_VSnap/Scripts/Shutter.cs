using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Shutter : MonoBehaviour
{
    [SerializeField] private Camera _captureCamera;

    [SerializeField]
    private CameraLayerController cameraLayerController;

    [SerializeField]
    private PreviewScreenShot preview;

    [SerializeField]
    private float imageAspect;

    private Texture2D screenShot;

    public Texture2D ScreenShot{
        
        get{
            return screenShot;
        }
    }


    public void TakeScreenshot()
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
        screenShot = new Texture2D(_captureCamera.pixelWidth, height, TextureFormat.ARGB32, false);

        Debug.Log("Screen Texture  " + height + " " + screenShot.width);
        
        screenShot.ReadPixels(new Rect(0, 0, screenShot.width, height), 0, 0, false);
        screenShot.Apply();

        try
        {
            string timestamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");

            NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(screenShot, "ShreenshotSample", $"screenshot_{timestamp}.jpg", (success, path) =>
            {
                // 保存終了時のコールバック
                if (preview != null){
                    preview.Preview(screenShot);
                }
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

    public async UniTask TakeCharactorAndBackground()
    {
        string layerName = "Charactor";
        int layer = LayerMask.NameToLayer(layerName);
        cameraLayerController.DisableLayer(_captureCamera, layer);
        TakeScreenshot();
        await UniTask.Yield();
        cameraLayerController.EnableLayer(_captureCamera, layer);
        TakeScreenshot();
    }

}
