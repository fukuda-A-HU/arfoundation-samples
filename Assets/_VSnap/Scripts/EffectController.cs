using UnityEngine;

public class EffectController : MonoBehaviour
{
    [SerializeField]
    GameObject rainEffect;
    public void WeatherEffect(Weather weather)
    {
        if (weather == Weather.Sunny)
        {
            Debug.Log("晴れ");
        }
        else if (weather == Weather.Cloudy)
        {
            Debug.Log("曇り");
        }
        else if (weather == Weather.Rainy)
        {
            Debug.Log("雨");
            rainEffect.SetActive(true);
        }
        else if (weather == Weather.Snowy)
        {
            Debug.Log("雪");
        }
    }
}
