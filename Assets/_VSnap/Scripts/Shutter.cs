using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shutter : MonoBehaviour
{
    [SerializeField] private Camera _captureCamera;

    public void TakeScreenshot()
    {
        // スクショ用の、ARカメラ描画結果を格納するRenderTextureを用意する
        RenderTexture rt = RenderTexture.GetTemporary(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);

        // 用意したRenderTextureに書き込む
        RenderTexture prevTarget = _captureCamera.targetTexture;
        _captureCamera.targetTexture = rt;
        _captureCamera.Render();
        _captureCamera.targetTexture = prevTarget;

        // RenderTextureのままでは保存できないので、Textureに変換する
        RenderTexture prevActive = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D(_captureCamera.pixelWidth, _captureCamera.pixelHeight, TextureFormat.ARGB32, false);
        screenShot.ReadPixels(new Rect(0, 0, screenShot.width, screenShot.height), 0, 0, false);

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
        }
        finally
        {
            // 最後にARカメラの描画先をスクリーンに戻す
            RenderTexture.ReleaseTemporary(rt);
            RenderTexture.active = prevActive;
        }
    }
}
