using TMPro;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor.UI;
#endif

public class SliderBase : Slider
{
    #region Inspector SerializedFields
    [SerializeField] private Image KnobImageRef;
    [SerializeField] private float Increment = 1;
    [SerializeField] private Color KnobSelectedColor;
    [SerializeField] private string[] ValueStringKeys;
    [SerializeField] public TextMeshProUGUI SliderTitle;
    [SerializeField] public TextMeshProUGUI SliderValue;
    [SerializeField] private string SliderValueFormat = "{0:0.0}";
    #endregion

    #region Public Members
    public bool CanChangeValue { get; private set; }
    public Action<float> OnFSGSliderValueChanged { get; set; }
    public Action<float> ActionToInvokeOnPointerRelease { get; set; }
    public enum ValueFormat { WholeNumbers, OneDecimal, TwoDecimal, None }
    #endregion

    #region Private Members
    private Color _initialKnobColor;
    private Navigation _noNavigation;
    private Navigation _initialNavigation;
    private float _previousSliderValue = 0;
    #endregion

    #region Unity Callbacks
    protected override void Start()
    {
        base.Start();
        Init();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        SetInteraction(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        SetInteraction(false);
        ActionToInvokeOnPointerRelease?.Invoke(base.value);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        ActionToInvokeOnPointerRelease?.Invoke(base.value);
    }

    private void OnSliderValueChanged(float sliderValue)
    {
        if (CanChangeValue)
        {
            UpdateTextOnValueChanged();
            OnFSGSliderValueChanged?.Invoke(sliderValue);
        }
        else
        {
            if (!Mathf.Approximately(sliderValue, _previousSliderValue))
            {
                Set(_previousSliderValue, true);
                //Debug.Log($"CannotChangeValue so will revert {name} value from {sliderValue} to {_previousSliderValue}");
            }
        }
    }
    #endregion

    #region Private Helpers
    private void Init()
    {
        onValueChanged.AddListener(OnSliderValueChanged);
        _initialKnobColor = KnobImageRef.color;
        _initialNavigation.mode = navigation.mode;
        _noNavigation.mode = Navigation.Mode.None;
        _previousSliderValue = base.value;
    }

    private void SetInteraction(bool state)
    {
        CanChangeValue = state;
        navigation = CanChangeValue ? _noNavigation : _initialNavigation;
        KnobImageRef.color = CanChangeValue ? KnobSelectedColor : _initialKnobColor;
    }

    private void UpdateTextOnValueChanged()
    {
        //Debug.Log($"UpdateTextOnValueChanged. {name} - {base.value}");
        if (SliderValue != null)
        {
            if (ValueStringKeys.Length > 0 && base.value < ValueStringKeys.Length)
            {
                SliderValue.text = string.Format(ValueStringKeys[(int)base.value]);
            }
            else
            {
                SliderValue.text = string.Format(SliderValueFormat, (base.value * Increment));
            }
        }

        var absDelta = Mathf.Abs(_previousSliderValue - base.value);
        if (absDelta > (Mathf.FloorToInt((maxValue - minValue) / 20f)))
        {
            _previousSliderValue = base.value;
        }
    }

    private string ConvertEnumToSliderFormatString(ValueFormat format)
    {
        switch (format)
        {
            case ValueFormat.WholeNumbers:
                return "{0:0}";
            case ValueFormat.OneDecimal:
                return "{0:0.0}";
            case ValueFormat.TwoDecimal:
                return "{0:0.00}";
            default:
                return "{0:0.0}";
        }
    }
    #endregion

    #region Internal API
    internal void DeselectSlider()
    {
        SetInteraction(false);
    }

    internal void ToggleSlider()
    {
        SetInteraction(!CanChangeValue);
    }

    internal void ForceSetValue(float newValue, bool sendCallback = true)
    {
        CanChangeValue = true;
        Set(newValue / Increment, sendCallback);
        _previousSliderValue = newValue;
        UpdateTextOnValueChanged();
        CanChangeValue = false;
    }

    internal void SetSliderProperties(float newMinValue, float newMaxValue, float newIncrement, string newTitle = "", ValueFormat format = ValueFormat.None)
    {
        Increment = newIncrement;

        minValue = Mathf.RoundToInt(newMinValue / Increment);
        maxValue = Mathf.RoundToInt(newMaxValue / Increment);
        if (format != ValueFormat.None)
        {
            SliderValueFormat = ConvertEnumToSliderFormatString(format);
        }

        if (!string.IsNullOrEmpty(newTitle))
        {
            SliderTitle.text = newTitle;
        }
    }
    #endregion
}


#if UNITY_EDITOR
[CustomEditor(typeof(SliderBase))]
public class SliderBaseEditor : SliderEditor
{
    private SerializedProperty _knobImageRef;
    private SerializedProperty _valueStringKeys;
    private SerializedProperty _sliderTitleText;
    private SerializedProperty _sliderValueText;
    private SerializedProperty _sliderValueFormat;
    private SerializedProperty _knobSelectedColor;
    protected override void OnEnable()
    {
        base.OnEnable();

        _knobImageRef = serializedObject.FindProperty("KnobImageRef");
        _sliderTitleText = serializedObject.FindProperty("SliderTitle");
        _sliderValueText = serializedObject.FindProperty("SliderValue");
        _valueStringKeys = serializedObject.FindProperty("ValueStringKeys");
        _sliderValueFormat = serializedObject.FindProperty("SliderValueFormat");
        _knobSelectedColor = serializedObject.FindProperty("KnobSelectedColor");
    }

    public override void OnInspectorGUI()
    {
        Color originalColor = GUI.contentColor;
        GUI.contentColor = Color.yellow;

        EditorGUILayout.PropertyField(_knobImageRef, false);
        EditorGUILayout.PropertyField(_sliderTitleText, false);
        EditorGUILayout.PropertyField(_sliderValueText, false);
        EditorGUILayout.PropertyField(_sliderValueFormat, false);
        EditorGUILayout.PropertyField(_knobSelectedColor, false);
        EditorGUILayout.PropertyField(_valueStringKeys);

        EditorGUILayout.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.Separator();
        EditorGUILayout.Space();
        GUI.contentColor = originalColor;
        base.OnInspectorGUI();
    }
}
#endif