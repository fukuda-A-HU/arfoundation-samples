using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;
using System.IO;

[RequireComponent(typeof(AROcclusionManager))]
public class ARKitDepthImage : MonoBehaviour
{
    private AROcclusionManager occlusionManager;

    void Start()
    {
        occlusionManager = GetComponent<AROcclusionManager>();
    }

    void Update()
    {
        if (occlusionManager != null)
        {
            if (occlusionManager.TryAcquireEnvironmentDepthCpuImage(out XRCpuImage depthImage))
            {
                using (depthImage)
                {
                    SaveDepthDataAsPNG(depthImage);
                }
            }
        }
    }
    private void SaveDepthDataAsPNG(XRCpuImage depthImage)
    {
        var conversionParams = new XRCpuImage.ConversionParams
        {
            // 入力画像の範囲をそのまま出力する
            inputRect = new RectInt(0, 0, depthImage.width, depthImage.height),
            // 深度画像のサイズをそのまま出力する
            outputDimensions = new Vector2Int(depthImage.width, depthImage.height),
            // ピクセルごとに1つのfloat値を持つテクスチャとする（深度画像のため1ピクセルごとに1つの深度値を持つ）
            outputFormat = TextureFormat.RFloat,
            // 左右を反転する（撮影者から見てiPhoneの長辺が地面に水平の角度かつカメラが左上に来るように持つ前提）
            transformation = XRCpuImage.Transformation.MirrorX
        };

        // 深度データを格納するバイト配列を用意する
        using NativeArray<byte> rawData = new(
            depthImage.GetConvertedDataSize(conversionParams), // conversionParamsから必要なデータサイズを求めて取得
            Allocator.Temp
        );

        // 深度データをバイト配列に変換して格納する
        depthImage.Convert(conversionParams, new NativeSlice<byte>(rawData));

        // 深度画像をグレースケール画像に変換
        Texture2D texture = new(
            depthImage.width,
            depthImage.height,
            TextureFormat.RFloat, // 深度データのため1ピクセルが1つの浮動小数点数を持つRloat型のテクスチャとする
            false
        );

        // テクスチャに深度データを格納する
        texture.LoadRawTextureData(rawData);

        // テクスチャを更新する
        texture.Apply();

        // テクスチャをグレースケールに変換する
        Texture2D grayScaleTexture = ConvertToGrayscale(texture);

        // PNGとして保存
        byte[] bytes = grayScaleTexture.EncodeToPNG();
        File.WriteAllBytes(Application.persistentDataPath + "/DepthImage.png", bytes);

        // テクスチャを破棄
        Destroy(texture);
        Destroy(grayScaleTexture);

        Debug.Log("Depth image saved as DepthImage.png");
    }

    private Texture2D ConvertToGrayscale(Texture2D texture)
    {
        Texture2D grayScaleTexture = new(
            texture.width,
            texture.height,
            TextureFormat.RGBA32,
            false
        );
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                float depthValue = texture.GetPixel(x, y).r; // 深度値は赤チャネルに格納されている
                grayScaleTexture.SetPixel(x, y, new Color(depthValue, depthValue, depthValue, 1)); // グレースケールに変換
            }
        }
        grayScaleTexture.Apply();
        return grayScaleTexture;
    }
}