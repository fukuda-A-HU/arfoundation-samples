using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class FItToRect : MonoBehaviour
{
    private RectTransform rawImageRectTransform;
    [SerializeField] private RectTransform refarenceTransform;
    [SerializeField] private RectTransform parentTransform;
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
        rawImageRectTransform = GetComponent<RectTransform>();

        if (refarenceTransform == null || rawImageRectTransform == null || parentTransform == null)
        {
            Debug.LogWarning("参照するオブジェクトまたは RawImage の RectTransform が見つかりません。");
            return;
        }

        float parentHeight = parentTransform.rect.height;
        float parentWidth = parentTransform.rect.width;

        // 親のアスペクト比
        float parentAspect = parentTransform.rect.width / parentTransform.rect.height;

        // Refarenceのアスペクト比
        float refAspect = refarenceTransform.rect.width / refarenceTransform.rect.height;

        // 親のアスペクト比に合わせる
        if (refAspect < parentAspect)
        {
            // 親の幅が余る場合は高さに合わせる
            float newHeight = parentHeight;
            float newWidth = newHeight * refAspect;

            rawImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
            rawImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        }
        else
        {
            // 親の高さが余る場合は幅に合わせる
            float newWidth = parentWidth;
            float newHeight = newWidth / refAspect;

            rawImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
            rawImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }
    }
}
