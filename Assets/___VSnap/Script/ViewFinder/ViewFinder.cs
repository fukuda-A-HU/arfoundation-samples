using UnityEngine;
using R3;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using Unity.Collections.LowLevel.Unsafe;
using System;


public class ViewFinder : MonoBehaviour
{
    [SerializeField] private Picture picture;
    [SerializeField] private ViewFinderView viewFinderView;
    [SerializeField] private ARCameraManager m_CameraManager;
    [SerializeField] private AROcclusionManager m_OcclusionManager;

    public SerializableReactiveProperty<Texture2D> cpuImageTexture = new SerializableReactiveProperty<Texture2D>();

    public SerializableReactiveProperty<Texture2D> depthImageTexture = new SerializableReactiveProperty<Texture2D>();


    private void Start()
    {
        cpuImageTexture.Subscribe(texture =>
        {
            viewFinderView.SetTexture(texture);
        }).AddTo(this);
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

        var m_Transformation = XRCpuImage.Transformation.MirrorX;

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

        RotateTexture90(cpuImageTexture.Value);
    }

    void RotateTexture90(Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;
        Color32[] originalPixels = texture.GetPixels32();
        Color32[] rotatedPixels = new Color32[originalPixels.Length];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                rotatedPixels[x * height + (height - y - 1)] = originalPixels[y * width + x];
            }
        }

        texture.Reinitialize(height, width);
        texture.SetPixels32(rotatedPixels);
        texture.Apply();
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
                var m_Transformation = XRCpuImage.Transformation.MirrorX;
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
