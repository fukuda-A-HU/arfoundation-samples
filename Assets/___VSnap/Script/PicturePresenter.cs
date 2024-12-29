using UnityEngine;
using R3;

public class PicturePresenter : MonoBehaviour
{
    [SerializeField] private PictureView view;
    [SerializeField] private Picture picture;

    private void Start()
    {
        view.shutterButton.onClick.AddListener(() => picture.TakePicture());
        view.previewImageButton.onClick.AddListener(() => picture.SetPreviewImage(false));

        picture.isPreviewing.Subscribe(isPreviewing =>
        {
            view.SetPreview(isPreviewing);
        });

        picture.pictureTexture.Subscribe(texture =>
        {
            view.previewImage.texture = texture;
        });
    }

    public void OnDestroy()
    {
        view.shutterButton.onClick.RemoveAllListeners();
        view.previewImageButton.onClick.RemoveAllListeners();
    }
}
