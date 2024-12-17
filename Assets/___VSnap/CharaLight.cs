using UnityEngine;
using R3;
using ObservableCollections;
using Cysharp.Threading.Tasks;

public class CharaLight : MonoBehaviour
{
    [SerializeField] private Vector2 lightHeightRange;
    public SerializableReactiveProperty<float> lightHeight;
    [SerializeField] private Vector2 lightAngleRange;
    public SerializableReactiveProperty<float> lightAngle;
    [SerializeField] private Vector2 envHueRange;
    public SerializableReactiveProperty<float> envHue;
    [SerializeField] private Vector2 envSatRange;
    public SerializableReactiveProperty<float> envSat;
    [SerializeField] private Vector2 envLumRange;
    public SerializableReactiveProperty<float> envLum;
    [SerializeField] private Vector2 dirHueRange;
    public SerializableReactiveProperty<float> dirHue;
    [SerializeField] private Vector2 dirSatRange;
    public SerializableReactiveProperty<float> dirSat;
    [SerializeField] private Vector2 dirLumRange;
    public SerializableReactiveProperty<float> dirLum;

    public void SetLightHeight(float lightHeight)
    {
        if (lightHeightRange.x <= lightHeight && lightHeight <= lightHeightRange.y)
        {
            this.lightHeight.Value = lightHeight;
        }
        else
        {
            Debug.LogError("lightHeight is out of range");
        }
    }

    public void SetLightAngle(float lightAngle)
    {
        if (lightAngleRange.x <= lightAngle && lightAngle <= lightAngleRange.y)
        {
            this.lightAngle.Value = lightAngle;
        }
        else
        {
            Debug.LogError("lightAngle is out of range");
        }
    }

    public void SetEnvHue(float envHue)
    {
        if (envHueRange.x <= envHue && envHue <= envHueRange.y)
        {
            this.envHue.Value = envHue;
        }
        else
        {
            Debug.LogError("envHue is out of range");
        }
    }

    public void SetEnvSat(float envSat)
    {
        if (envSatRange.x <= envSat && envSat <= envSatRange.y)
        {
            this.envSat.Value = envSat;
        }
        else
        {
            Debug.LogError("envSat is out of range");
        }
    }

    public void SetEnvLum(float envLum)
    {
        if (envLumRange.x <= envLum && envLum <= envLumRange.y)
        {
            this.envLum.Value = envLum;
        }
        else
        {
            Debug.LogError("envLum is out of range");
        }
    }

    public void SetDirHue(float dirHue)
    {
        if (dirHueRange.x <= dirHue && dirHue <= dirHueRange.y)
        {
            this.dirHue.Value = dirHue;
        }
        else
        {
            Debug.LogError("dirHue is out of range");
        }
    }

    public void SetDirSat(float dirSat)
    {
        if (dirSatRange.x <= dirSat && dirSat <= dirSatRange.y)
        {
            this.dirSat.Value = dirSat;
        }
        else
        {
            Debug.LogError("dirSat is out of range");
        }
    }

    public void SetDirLum(float dirLum)
    {
        if (dirLumRange.x <= dirLum && dirLum <= dirLumRange.y)
        {
            this.dirLum.Value = dirLum;
        }
        else
        {
            Debug.LogError("dirLum is out of range");
        }
    }
}
