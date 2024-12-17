using UnityEngine;
using CesiumForUnity;

public class SyncOrigin : MonoBehaviour
{
    [SerializeField]
    private CesiumGlobeAnchor originAnchor;

    [SerializeField]
    private CesiumGlobeAnchor controllAnchor;


    void Update(){
        originAnchor.longitudeLatitudeHeight = controllAnchor.longitudeLatitudeHeight;
    }

}
