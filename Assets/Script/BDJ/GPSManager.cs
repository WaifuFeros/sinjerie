using UnityEngine;
using System.Collections;
using TMPro;

public class GPSManager : MonoBehaviour
{
    public TextMeshProUGUI positionText;

    public float latitude;
    public float longitude;

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
            Debug.Log("GPS désactivé");
            if (positionText != null)
                positionText.text = "GPS désactivé";
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            if (positionText != null)
                positionText.text = "Recherche GPS...";
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.Log("GPS indisponible");
            if (positionText != null)
                positionText.text = "GPS indisponible";
            yield break;
        }

        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;

        Debug.Log($"GPS OK : {latitude} / {longitude}");

        if (positionText != null)
        {
            positionText.text =
                $"Latitude : {latitude:F5}\nLongitude : {longitude:F5}";
        }

        Input.location.Stop();
    }
}
