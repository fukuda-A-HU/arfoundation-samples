using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ExtendedSlider : Slider, IPointerUpHandler
{
    [System.Serializable]
    public class SliderReleaseEvent : UnityEvent<float> { }

    // スライダーを離したときに発火するイベント
    public SliderReleaseEvent onRelease = new SliderReleaseEvent();

    // PointerUpイベントを検知
    public void OnPointerUp(PointerEventData eventData)
    {
        onRelease.Invoke(value);
    }
}
