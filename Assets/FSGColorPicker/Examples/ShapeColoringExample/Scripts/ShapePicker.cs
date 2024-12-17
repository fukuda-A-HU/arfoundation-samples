using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ShapePicker : MonoBehaviour
{
    [SerializeField] private ColorPicker ColorPickerRef;
    [SerializeField] private TextMeshProUGUI SelectedShapeText;

    [Header("Selectable Shapes")]
    [SerializeField] private Button Heart;
    [SerializeField] private Button Circle;
    [SerializeField] private Button Square;

    private Dictionary<Button, Image> _cachedImages;
    private Image _imageOfSelectedShape;

    private void Start()
    {
        _cachedImages = new Dictionary<Button, Image>();

        Heart.onClick.AddListener(OnHeartClicked);
        Circle.onClick.AddListener(OnCircleClicked);
        Square.onClick.AddListener(OnSquareClicked);

        ColorPickerRef.OnColorValueChanged += ChangeColorOfSelectedShape;
    }

    private void OnHeartClicked()
    {
        ColorPickerRef.Show(true);
        SelectShape(Heart);
    }
    private void OnCircleClicked()
    {
        ColorPickerRef.Show(true);
        SelectShape(Circle);
    }

    private void OnSquareClicked()
    {
        ColorPickerRef.Show(true, true);
        SelectShape(Square);
    }

    private void ChangeColorOfSelectedShape(Color newColor)
    {
        _imageOfSelectedShape.color = newColor;
    }

    private void SelectShape(Button square)
    {
        if (_cachedImages.ContainsKey(square))
        {
            _imageOfSelectedShape = _cachedImages[square];
        }
        else
        {
            Image newImage = square.GetComponent<Image>();
            _cachedImages.Add(square, newImage);
            _imageOfSelectedShape = newImage;
        }

        ColorPickerRef.UpdateSelectedColor(_imageOfSelectedShape.color, false);
        // SelectedShapeText.text = square.name;
    }
}
