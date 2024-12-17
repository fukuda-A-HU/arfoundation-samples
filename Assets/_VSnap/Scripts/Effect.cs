using UnityEngine;

public class Effect : MonoBehaviour
{
    [SerializeField]
    GameObject rainEffect;
    void OnTriggerEnter(Collider col)
    {
        var pointController = col.gameObject.GetComponent<PointController>();

        if (pointController.weather == Weather.Sunny)
        {
            Debug.Log("晴れ");
            rainEffect.SetActive(false);
            RenderSettings.ambientLight = new Color(1f, 1f, 1f);
        }
        else if (pointController.weather == Weather.Cloudy)
        {
            Debug.Log("曇り");
            rainEffect.SetActive(false);
            RenderSettings.ambientLight = new Color(0.5f, 0.5f, 0.5f);
        }
        else if (pointController.weather == Weather.Rainy)
        {
            Debug.Log("雨");
            rainEffect.SetActive(true);
            RenderSettings.ambientLight = new Color(0.7f, 0.7f, 0.7f);
        }
        else if (pointController.weather == Weather.Snowy)
        {
            Debug.Log("雪");
            rainEffect.SetActive(false);
            RenderSettings.ambientLight = new Color(0.8f, 0.8f, 0.8f);
        }
        
    }
}
