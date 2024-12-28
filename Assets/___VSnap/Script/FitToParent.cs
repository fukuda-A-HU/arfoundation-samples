using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(RawImage))]
public class FitToParent : MonoBehaviour
{
    private RectTransform parentRectTransform;
    private RectTransform rawImageRectTransform;
    [SerializeField] private bool ExecuteAlways = true;

    void Start()
    {
        AdjustSize();
    }

    // エディタ上での変更を検知
#if UNITY_EDITOR
    void Update()
    {
        if (ExecuteAlways)
        {
            AdjustSize();
        }
    }
#endif

    public void AdjustSize()
    {
        // 親オブジェクトの RectTransform を取得
        if (transform.parent != null)
        {
            parentRectTransform = transform.parent.GetComponent<RectTransform>();
        }
        rawImageRectTransform = GetComponent<RectTransform>();

        if (parentRectTransform == null || rawImageRectTransform == null)
        {
            Debug.LogWarning("親オブジェクトまたは RawImage の RectTransform が見つかりません。");
            return;
        }

        // 親の幅と高さ
        float parentWidth = parentRectTransform.rect.width;
        float parentHeight = parentRectTransform.rect.height;

        // RawImage の元のアスペクト比
        float rawImageAspect = rawImageRectTransform.rect.width / rawImageRectTransform.rect.height;

        // 親のアスペクト比に合わせる
        if (parentWidth / parentHeight > rawImageAspect)
        {
            // 親の幅が余る場合は高さに合わせる
            float newHeight = parentHeight;
            float newWidth = newHeight * rawImageAspect;

            rawImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
            rawImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        }
        else
        {
            // 親の高さが余る場合は幅に合わせる
            float newWidth = parentWidth;
            float newHeight = newWidth / rawImageAspect;

            rawImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
            rawImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }
    }
}
