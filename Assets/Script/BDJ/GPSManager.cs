using UnityEngine;
using System.Collections;
using TMPro;

public class GPSManager : MonoBehaviour
{
    public TextMeshProUGUI positionText;

    public float latitude;
    public float longitude;
    public bool gpsReady = false;

    IEnumerator Start()
    {
#if UNITY_ANDROID
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(
            UnityEngine.Android.Permission.FineLocation))
        {
            UnityEngine.Android.Permission.RequestUserPermission(
                UnityEngine.Android.Permission.FineLocation);
        }
#endif

        if (!Input.location.isEnabledByUser)
        {
            positionText.text = "GPS désactivé";
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            positionText.text = "Recherche GPS...";
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            positionText.text = "GPS indisponible";
            yield break;
        }

        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;
        gpsReady = true;

        positionText.text =
            $"Lat: {latitude:F5}\nLon: {longitude:F5}";
    }
}
