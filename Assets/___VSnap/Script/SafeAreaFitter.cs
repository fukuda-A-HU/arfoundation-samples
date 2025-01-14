using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    private void Awake()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Rect deviceSafeArea = Screen.safeArea;

        Vector2 anchorMin = deviceSafeArea.position;
        Vector2 anchorMax = deviceSafeArea.position + deviceSafeArea.size;

        anchorMin.x /= screenSize.x;
        anchorMax.x /= screenSize.x;
        anchorMin.y /= screenSize.y;
        anchorMax.y /= screenSize.y;

        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
}