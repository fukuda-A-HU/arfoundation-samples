using UnityEngine;
using UnityEngine.UI;

public class PictureView : MonoBehaviour
{
    public Button shutterButton;
    public RawImage previewImage;
    public Button previewImageButton;
    public RectTransform previewImageParent;

    [SerializeField] private RawImage[] rawImages;
    [SerializeField] private Image[] images;
    [SerializeField] private Button[] buttons;
    [SerializeField] private GameObject[] gameObjects;

    public void SetPreview(bool isPreviewing)
    {
        // previewImageParent.gameObject.SetActive(isPreviewing);
        foreach (var rawImage in rawImages)
        {
            rawImage.enabled = isPreviewing;
        }
        foreach (var image in images)
        {
            image.enabled = isPreviewing;
        }
        foreach (var button in buttons)
        {
            button.enabled = isPreviewing;
        }
        foreach (var gameObject in gameObjects)
        {
            gameObject.SetActive(isPreviewing);
        }
    }
}
