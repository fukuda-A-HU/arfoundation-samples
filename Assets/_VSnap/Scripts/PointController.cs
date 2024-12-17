using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PointController : MonoBehaviour
{
    [SerializeField]
    private RawImage rawImage;
    public Texture2D picture;

    public Texture2D Picture
    {
        get
        {
            return picture;
        }
        set
        {
            picture = value;
            rawImage.texture = picture;

        }
    }   

    [SerializeField]
    private TextMeshProUGUI titleText;

    public string review;

    public string Review
    {
        get
        {
            return review;
        }
        set
        {
            review = value;
            titleText.text = review;
        }
    }

    public Weather weather;

    public Weather Weather
    {
        get
        {
            return weather;
        }
        set
        {
            weather = value;
        }
    }

    public string nearsides;


}
