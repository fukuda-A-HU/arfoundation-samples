using UnityEngine;
using R3;

public class PicturePresenter : MonoBehaviour
{
    [SerializeField] private PictureView view;
    [SerializeField] private Picture picture;
    [SerializeField] private AR ar;

    private void Start()
    {
        view.shutterButton.onClick.AddListener(() => 
        {
            var state = ar.isOn.Value;
            if (state)
            {
                ar.isOn.Value = false;
            }
            picture.TakePicture();
            if (state)
            {
                ar.isOn.Value = true;
            }
        });

        view.previewImageButton.onClick.AddListener(() => picture.SetPreviewImage(false));

        picture.isPreviewing.Subscribe(isPreviewing =>
        {
            view.SetPreview(isPreviewing);
        }).AddTo(this);

        picture.pictureTexture.Subscribe(texture =>
        {
            view.previewImage.texture = texture;
        }).AddTo(this);
    }

    public void OnDestroy()
    {
        view.shutterButton.onClick.RemoveAllListeners();
        view.previewImageButton.onClick.RemoveAllListeners();
    }
}
