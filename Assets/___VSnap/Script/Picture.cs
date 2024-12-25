using UnityEngine;
using System;
using System.IO;
using R3;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using Unity.Collections.LowLevel.Unsafe;

public class Picture : MonoBehaviour
{
    [SerializeField] private Camera _captureCamera;
    [SerializeField] private ARCameraManager m_CameraManager;
    [SerializeField] private AROcclusionManager m_OcclusionManager;
    public SerializableReactiveProperty<bool> isPreviewing = new SerializableReactiveProperty<bool>();
    public SerializableReactiveProperty<Texture2D> pictureTexture = new SerializableReactiveProperty<Texture2D>();
    public SerializableReactiveProperty<Texture2D> cpuImageTexture = new SerializableReactiveProperty<Texture2D>();

    public SerializableReactiveProperty<Texture2D> depthImageTexture = new SerializableReactiveProperty<Texture2D>();
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
        Debug.Log("CPU Texture  " + cpuImageTexture.Value.height + " " + cpuImageTexture.Value.width);
        
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



    //  =---------

    delegate bool TryAcquireDepthImageDelegate(out XRCpuImage image);


    void OnEnable()
    {
        m_CameraManager.frameReceived += OnCameraFrameReceived;
    }

    void OnDisable()
    {
        m_CameraManager.frameReceived -= OnCameraFrameReceived;
    }

    void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        UpdateCameraImage();
        // UpdateDepthImage(m_OcclusionManager.TryAcquireHumanDepthCpuImage, m_RawHumanDepthImage);
        // UpdateDepthImage(m_OcclusionManager.TryAcquireHumanStencilCpuImage, m_RawHumanStencilImage);
        UpdateDepthImage(m_OcclusionManager.TryAcquireEnvironmentDepthCpuImage);
        // UpdateDepthImage(m_OcclusionManager.TryAcquireEnvironmentDepthConfidenceCpuImage, m_RawEnvironmentDepthConfidenceImage);
    }

    unsafe void UpdateCameraImage()
    {
        // Attempt to get the latest camera image. If this method succeeds,
        // it acquires a native resource that must be disposed (see below).
        if (!m_CameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            return;
        }

        // Display some information about the camera image
        // m_ImageInfo.text = string.Format(
        //    "Image info:\n\twidth: {0}\n\theight: {1}\n\tplaneCount: {2}\n\ttimestamp: {3}\n\tformat: {4}",
        //      image.width, image.height, image.planeCount, image.timestamp, image.format);

        // Once we have a valid XRCpuImage, we can access the individual image "planes"
        // (the separate channels in the image). XRCpuImage.GetPlane provides
        // low-overhead access to this data. This could then be passed to a
        // computer vision algorithm. Here, we will convert the camera image
        // to an RGBA texture and draw it on the screen.

        // Choose an RGBA format.
        // See XRCpuImage.FormatSupported for a complete list of supported formats.
        const TextureFormat format = TextureFormat.RGBA32;

        if (cpuImageTexture.Value == null || cpuImageTexture.Value.width != image.width || cpuImageTexture.Value.height != image.height)
            cpuImageTexture.Value = new Texture2D(image.width, image.height, format, false);

        // Convert the image to format, flipping the image across the Y axis.
        // We can also get a sub rectangle, but we'll get the full image here.

        var m_Transformation = XRCpuImage.Transformation.MirrorY;

        var conversionParams = new XRCpuImage.ConversionParams(image, format, m_Transformation);

        // Texture2D allows us write directly to the raw texture data
        // This allows us to do the conversion in-place without making any copies.
        var rawTextureData = cpuImageTexture.Value.GetRawTextureData<byte>();
        try
        {
            image.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
        }
        finally
        {
            // We must dispose of the XRCpuImage after we're finished
            // with it to avoid leaking native resources.
            image.Dispose();
        }

        // Apply the updated texture data to our texture
        cpuImageTexture.Value.Apply();
    }

    /// <summary>
    /// Calls <paramref name="tryAcquireDepthImageDelegate"/> and renders the resulting depth image contents to <paramref name="rawImage"/>.
    /// </summary>
    /// <param name="tryAcquireDepthImageDelegate">The method to call to acquire a depth image.</param>
    /// <param name="rawImage">The Raw Image to use to render the depth image to the screen.</param>
    void UpdateDepthImage(TryAcquireDepthImageDelegate tryAcquireDepthImageDelegate)
    {
        if (tryAcquireDepthImageDelegate(out XRCpuImage cpuImage))
        {
            // XRCpuImages, if successfully acquired, must be disposed.
            // You can do this with a using statement as shown below, or by calling its Dispose() method directly.
            using (cpuImage)
            {
                var m_Transformation = XRCpuImage.Transformation.MirrorY;
                depthImageTexture.Value = UpdateRawImage(depthImageTexture.Value, cpuImage, m_Transformation);
            }
        }
        else
        {
            
        }
    }

    static Texture2D UpdateRawImage(Texture2D texture, XRCpuImage cpuImage, XRCpuImage.Transformation transformation)
    {
        // Get the texture associated with the UI.RawImage that we wish to display on screen.
        // var texture = _texture;

        // If the texture hasn't yet been created, or if its dimensions have changed, (re)create the texture.
        // Note: Although texture dimensions do not normally change frame-to-frame, they can change in response to
        //    a change in the camera resolution (for camera images) or changes to the quality of the human depth
        //    and human stencil buffers.
        if (texture == null || texture.width != cpuImage.width || texture.height != cpuImage.height)
        {
            texture = new Texture2D(cpuImage.width, cpuImage.height, cpuImage.format.AsTextureFormat(), false);
            // rawImage.texture = texture;
        }

        // For display, we need to mirror about the vertical access.
        var conversionParams = new XRCpuImage.ConversionParams(cpuImage, cpuImage.format.AsTextureFormat(), transformation);

        // Get the Texture2D's underlying pixel buffer.
        var rawTextureData = texture.GetRawTextureData<byte>();

        // Make sure the destination buffer is large enough to hold the converted data (they should be the same size)
        Debug.Assert(rawTextureData.Length == cpuImage.GetConvertedDataSize(conversionParams.outputDimensions, conversionParams.outputFormat),
            "The Texture2D is not the same size as the converted data.");

        // Perform the conversion.
        cpuImage.Convert(conversionParams, rawTextureData);

        // "Apply" the new pixel data to the Texture2D.
        texture.Apply();

        // Make sure it's enabled.
        return texture;
    }
}
