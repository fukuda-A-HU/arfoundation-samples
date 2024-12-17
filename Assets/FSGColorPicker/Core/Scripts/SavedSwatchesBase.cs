using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SavedSwatchesBase : MonoBehaviour
{
    [SerializeField] private ColorPicker ColorPickerRef;
    [SerializeField] private RectTransform ClampAreaRect;

    public Image ImageRef; 
    public Button ButtonRef;
    public float YOffset = 40;
    public RectTransform SwatchRect;

    #region Unity Callbacks
    private void OnEnable()
    {
        ButtonRef.onClick.AddListener(OnSwatchButtonClicked);
    }

    private void OnDisable()
    {
        ButtonRef.onClick.RemoveListener(OnSwatchButtonClicked);
    }
#endregion

    public void OnSwatchButtonClicked()
    {
        var index = Array.IndexOf(ColorPickerRef.SavedSwatchesBases, this);
        EventSystem.current.SetSelectedGameObject(ColorPickerRef.ApplySwatchButton.gameObject);
        if (index == ColorPickerRef.CurrentSwatchIndex) //If user clicks on swatch that is already selected
        {
            var state = ColorPickerRef.OptionsMenuGo.activeSelf;
            ColorPickerRef.OptionsMenuGo.SetActive(!state);
            if (state)
            {
                EventSystem.current.SetSelectedGameObject(gameObject);
            }
        }
        else
        {
            ColorPickerRef.CurrentSwatchIndex = index;
            ColorPickerRef.OptionsMenuRectTransform.localPosition = new Vector2(SwatchRect.localPosition.x, SwatchRect.localPosition.y + YOffset);
            ClampOptionsMenuPosition();
            ColorPickerRef.OptionsMenuGo.SetActive(true);
        }
    }

    internal void ClampOptionsMenuPosition()
    {
        var menuRectTransform = ColorPickerRef.OptionsMenuRectTransform;
        Vector3 pos = menuRectTransform.localPosition;

        Vector3 minPos = ClampAreaRect.rect.min - menuRectTransform.rect.min;
        Vector3 maxPos = ClampAreaRect.rect.max - menuRectTransform.rect.max;

        pos.x = Mathf.Clamp(menuRectTransform.localPosition.x, minPos.x, maxPos.x);
        pos.y = Mathf.Clamp(menuRectTransform.localPosition.y, minPos.y, maxPos.y);

        menuRectTransform.localPosition = pos;
    }
}