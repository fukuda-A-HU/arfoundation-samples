using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ColorPicker : MonoBehaviour
{
    [Header("Results")]
    [SerializeField] private Image PreviewColorImage;

    [Header("Hue")]
    [SerializeField] private Image HueImage;
    public SliderBase HueSlider;

    [Header("Saturation")]
    [SerializeField] private Image SatImage;
    [SerializeField] private SliderBase SatSlider;

    [Header("Luminance")]
    [SerializeField] private Image LuminanceImage;
    [SerializeField] private SliderBase LuminanceSlider;

    [Header("Alpha")]
    [SerializeField] private Image AlphaImage;
    [SerializeField] private SliderBase AlphaSlider;

    [Header("Swatches")]
    public Button ApplySwatchButton;
    public GameObject OptionsMenuGo;
    public SwatchesData SwatchesData;
    public RectTransform OptionsMenuRectTransform;
    public SavedSwatchesBase[] SavedSwatchesBases;
    [SerializeField] private Button SaveSwatchButton;
    [SerializeField] private List<SwatchesData.SwatchData> DefaultSwatchData = new();

    #region Private Members
    private bool _isInit;

    private Texture2D _lumTexture;
    private Texture2D _alphaTexture;
    private Texture2D _saturationTexture;

    [Header("Colors")]
    [SerializeField] private Color[] _hueColors;

    public Color[] SaturationColors = new Color[] {
            Color.black,
            Color.black,
            Color.white,
            Color.red
        };
    #endregion

    #region Public Members/Properties
    public Action<Color> OnColorValueChanged;
    public int CurrentSwatchIndex { get; set; }
    public Color SelectedColor { get; private set; }
    public bool IsShown { get { return gameObject.activeSelf; } }
    private float NormalizedHueValue { get { return HueSlider.value / HueSlider.maxValue; } }
    private float NormalizedSatValue { get { return SatSlider.value / SatSlider.maxValue; } }
    private float NormalizedAlphaValue { get { return AlphaSlider.value / AlphaSlider.maxValue; } }
    private float NormalizedLumValue { get { return LuminanceSlider.value / LuminanceSlider.maxValue; } }

    public string SwatchesDataFilePath
    {
        get
        {
            return string.Format("{0}{1}", Application.persistentDataPath, string.Format(COLOR_PICKER_SWATCHES_PATH_FORMAT));
        }
    }
    #endregion

    #region Private Constants
    private readonly Color INITIAL_COLOR = Color.HSVToRGB(0f, 1f, 0.55f);
    private readonly string COLOR_PICKER_SWATCHES_PATH_FORMAT = "/Customisations/SwatchData.dat";
    #endregion

    #region Unity Callbacks
    private void OnEnable()
    {
        HueSlider.OnFSGSliderValueChanged += OnHueSliderValueChanged;
        SatSlider.OnFSGSliderValueChanged += OnSatSliderValueChanged;
        AlphaSlider.OnFSGSliderValueChanged += OnAlphaValueSliderChanged;
        LuminanceSlider.OnFSGSliderValueChanged += OnLuminanceSliderValueChanged;

        SaveSwatchButton.onClick.AddListener(OnSaveSwatchButtonClicked);
        ApplySwatchButton.onClick.AddListener(OnApplySwatchButtonClicked);
    }

    private void OnDisable()
    {
        HueSlider.OnFSGSliderValueChanged -= OnHueSliderValueChanged;
        SatSlider.OnFSGSliderValueChanged -= OnSatSliderValueChanged;
        AlphaSlider.OnFSGSliderValueChanged -= OnAlphaValueSliderChanged;
        LuminanceSlider.OnFSGSliderValueChanged -= OnLuminanceSliderValueChanged;

        SaveSwatchButton.onClick.RemoveListener(OnSaveSwatchButtonClicked);
        ApplySwatchButton.onClick.RemoveListener(OnApplySwatchButtonClicked);
    }
    #endregion

    #region Internal API
    internal void Show(bool state, bool showAlphaChannel = false)
    {
        if (state && !_isInit)
        {
            Init();
        }

        gameObject.SetActive(state);
        AlphaSlider.gameObject.SetActive(showAlphaChannel);
        //Debug.LogFormat($"SHOWING COLOR PICKER: {state}. Alpha Channel: {showAlphaChannel}");
    }

    internal void UpdateSelectedColor(Color color, bool sendCallback = true, bool forceUpdate = false)
    {
        if (!IsShown && !forceUpdate)
        {
            return;
        }

        SelectedColor = color;
        Color.RGBToHSV(SelectedColor, out float hue, out float satValue, out float lumValue);
        HueSlider.ForceSetValue(hue * HueSlider.maxValue, sendCallback);
        SatSlider.ForceSetValue(satValue * SatSlider.maxValue, sendCallback);
        LuminanceSlider.ForceSetValue(lumValue * LuminanceSlider.maxValue, sendCallback);
        AlphaSlider.ForceSetValue(SelectedColor.a * AlphaSlider.maxValue, sendCallback);
        PreviewColorImage.color = SelectedColor;

        UpdateHueValue();

        UpdateSatSprite();
        UpdateLumSprite();
        UpdateAlphaSprite();
        //Debug.Log($"UpdateSelectedColor Hue: {hue}. Sat: {NormalizedSatValue}. Lum: {NormalizedLumValue}. SendCallback: {sendCallback} ==> {SelectedColor}");
    }
    #endregion

    #region Init
    private void Init()
    {
        _isInit = true;
        CreateHueSprite();

        _saturationTexture = new Texture2D(2, 2);
        SatImage.sprite = Sprite.Create(_saturationTexture, new Rect(0.5f, 0, 1, 1), new Vector2(0.5f, 0.5f));
        UpdateSatSprite();

        _lumTexture = new Texture2D(2, 2);
        LuminanceImage.sprite = Sprite.Create(_lumTexture, new Rect(0.5f, 0, 1, 1), new Vector2(0.5f, 0.5f));
        UpdateLumSprite();

        _alphaTexture = new Texture2D(2, 2);
        AlphaImage.sprite = Sprite.Create(_alphaTexture, new Rect(0.5f, 0, 1, 1), new Vector2(0.5f, 0.5f));
        UpdateAlphaSprite();

        UpdateSelectedColor(INITIAL_COLOR, false, true);
        LoadDataFromDisk();
        ApplySavedOrDefaultColors();
        PositionSwatchOptionsMenu();
    }

    private void ApplySavedOrDefaultColors()
    {
        for (int i = 0; i < SavedSwatchesBases.Length; i++)
        {
            if (SwatchesData.Swatches.ElementAtOrDefault(i) != null)
            {
                SwatchesData.SwatchData swatchData = SwatchesData.Swatches[i];
                UpdateSelectedColor(swatchData.SwatchColor);
                SavedSwatchesBases[i].ImageRef.color = swatchData.SwatchColor;
            }
            else
            {
                SavedSwatchesBases[i].ImageRef.color = DefaultSwatchData[i].SwatchColor;
                SwatchesData.Swatches.Add(DefaultSwatchData[i]);
            }
        }
    }

    private void CreateHueSprite()
    {
        var hueTex = new Texture2D(_hueColors.Length, 1);
        for (int i = 0; i < _hueColors.Length; i++)
        {
            hueTex.SetPixel(i, 0, _hueColors[i]);
        }
        hueTex.Apply();

        HueImage.sprite = Sprite.Create(hueTex, new Rect(0.5f, 0, _hueColors.Length - 1, 1), new Vector2(0.5f, 0.5f));
    }

    private void UpdateSatSprite()
    {
        Color leftSideColor = Color.Lerp(Color.black, Color.white, (LuminanceSlider.value / LuminanceSlider.maxValue));
        Color rightSideColor = Color.Lerp(Color.black, SaturationColors[3], (LuminanceSlider.value / LuminanceSlider.maxValue));

        _saturationTexture.SetPixel(0, 0, leftSideColor);
        _saturationTexture.SetPixel(0, 1, leftSideColor);
        _saturationTexture.SetPixel(1, 0, rightSideColor);
        _saturationTexture.SetPixel(1, 1, rightSideColor);
        _saturationTexture.Apply();
    }

    private void UpdateLumSprite()
    {
        Color leftSideColor = Color.black;
        Color rightSideColor = Color.Lerp(Color.white, SaturationColors[3], (SatSlider.value / SatSlider.maxValue));

        _lumTexture.SetPixel(0, 0, leftSideColor);
        _lumTexture.SetPixel(0, 1, leftSideColor);
        _lumTexture.SetPixel(1, 0, rightSideColor);
        _lumTexture.SetPixel(1, 1, rightSideColor);
        _lumTexture.Apply();
    }

    private void UpdateAlphaSprite()
    {
        Color leftSideColor = Color.clear;
        Color rightSideColor = Color.Lerp(Color.white, SaturationColors[3], (SatSlider.value / SatSlider.maxValue)) * Color.Lerp(Color.black, Color.white, (LuminanceSlider.value / LuminanceSlider.maxValue));

        _alphaTexture.SetPixel(0, 0, leftSideColor);
        _alphaTexture.SetPixel(0, 1, leftSideColor);
        _alphaTexture.SetPixel(1, 0, rightSideColor);
        _alphaTexture.SetPixel(1, 1, rightSideColor);
        _alphaTexture.Apply();
    }

    private void PositionSwatchOptionsMenu()
    {
        if (SavedSwatchesBases[0] != null)
        {
            var firstSwatchRect = SavedSwatchesBases[0].SwatchRect;
            OptionsMenuRectTransform.localPosition = new Vector2(firstSwatchRect.localPosition.x, firstSwatchRect.localPosition.y + SavedSwatchesBases[0].YOffset);
            SavedSwatchesBases[0].ClampOptionsMenuPosition();
        }
    }
    #endregion

    #region Button Callbacks
    private void OnApplySwatchButtonClicked()
    {
        if (SwatchesData.Swatches.ElementAtOrDefault(CurrentSwatchIndex) != null)
        {
            UpdateSelectedColor(SwatchesData.Swatches[CurrentSwatchIndex].SwatchColor, true);
        }

        HideSwatchOptionsMenu();
    }

    private void OnSaveSwatchButtonClicked()
    {
        var currentColor = GetColorFromHSV();
        if (SwatchesData.Swatches.ElementAtOrDefault(CurrentSwatchIndex) != null)
        {
            SwatchesData.Swatches[CurrentSwatchIndex].SwatchColor = currentColor;
        }
        else
        {
            SwatchesData.SwatchData newSwatch = new()
            {
                SwatchColor = currentColor,
                SwatchNumber = CurrentSwatchIndex
            };

            SwatchesData.Swatches.Add(newSwatch);
        }

        SavedSwatchesBases[CurrentSwatchIndex].ImageRef.color = currentColor;
        SaveToDisk();

        HideSwatchOptionsMenu();
    }
    #endregion

    #region Internal API
    internal void HideSwatchOptionsMenu()
    {
        OptionsMenuGo.SetActive(false);
        EventSystem.current.SetSelectedGameObject(SavedSwatchesBases[CurrentSwatchIndex].gameObject);
    }
    #endregion

    #region Slider Callbacks
    private void OnHueSliderValueChanged(float hueSliderValue)
    {
        UpdateHueValue();
        UpdateResultantColor();
    }

    private void OnLuminanceSliderValueChanged(float luminanceValue)
    {
        UpdateResultantColor();
    }

    private void OnSatSliderValueChanged(float satSliderValue)
    {
        UpdateResultantColor();
    }

    private void OnAlphaValueSliderChanged(float alphaSliderValue)
    {
        UpdateResultantColor();
    }
    #endregion

    #region Result
    private Color GetColorFromHSV()
    {
        var resultColor = Color.HSVToRGB(NormalizedHueValue, NormalizedSatValue, NormalizedLumValue);
        resultColor.a = NormalizedAlphaValue;
        //Debug.LogWarning($"NormalizedHueValue : {NormalizedHueValue }. NormalizedSatValue: {NormalizedSatValue}. NormalizedLumValue: {NormalizedLumValue}. Sat3{SaturationColors[3]} ==>> {resultColor}");
        return resultColor;
    }

    private void UpdateResultantColor()
    {
        var resultColor = GetColorFromHSV();
        PreviewColorImage.color = resultColor;

        UpdateSatSprite();
        UpdateLumSprite();
        UpdateAlphaSprite();

        OnColorValueChanged?.Invoke(resultColor);
    }

    private void UpdateHueValue()
    {
        var sliderBaseValue = HueSlider.maxValue / (_hueColors.Length - 1f); //E.g. 20
        var normalizedSliderValue = HueSlider.value / sliderBaseValue; //E.g. 0 - 260
        var leftColorIndex = Mathf.FloorToInt(normalizedSliderValue);
        var nextColorIndex = Mathf.Clamp(leftColorIndex + 1, leftColorIndex, _hueColors.Length - 1);
        var colorDelta = normalizedSliderValue - leftColorIndex;
        var hueColor = Color.Lerp(_hueColors[leftColorIndex], _hueColors[nextColorIndex], colorDelta);
        //Debug.LogFormat($"HueSliderValue {sliderBaseValue}. Normalized: {normalizedSliderValue}. LeftColorIndex {leftColorIndex}. Next: {nextColorIndex}. Delta: {colorDelta} ==> {hueColor}");

        SaturationColors[3] = hueColor;
    }
    #endregion

    #region Saving
    private void LoadDataFromDisk()
    {
        SwatchesData = new SwatchesData();
        if (File.Exists(SwatchesDataFilePath))
        {
            var serializedData = File.ReadAllText(SwatchesDataFilePath);
            JsonUtility.FromJsonOverwrite(serializedData, SwatchesData);
            Debug.Log("SwatchesData exists");
        }
        else
        {
            DirectoryInfo parent = Directory.GetParent(SwatchesDataFilePath);
            Directory.CreateDirectory(parent.FullName);
            Debug.Log($"No SwatchesData found. Creating directory at {parent.FullName}");
        }
    }

    private void SaveToDisk()
    {
        var serializedData = JsonUtility.ToJson(SwatchesData);
        File.WriteAllText(SwatchesDataFilePath, serializedData);
        Debug.Log("#CP - Saved Swatch Data");
    }
    #endregion
}