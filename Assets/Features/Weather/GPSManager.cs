using UnityEngine;
using System.Collections;
using TMPro;

public class GPSManager : MonoBehaviour
{
    public TextMeshProUGUI positionText;

    public float latitude;
    public float longitude;
    public bool gpsReady = false;

    [Header("EDITOR DEBUG")]
    public bool simulateInEditor = true;
    public float editorLatitude = 45.6484f;   // Angoulême
    public float editorLongitude = 0.1562f;

    IEnumerator Start()
    {
#if UNITY_EDITOR
        if (simulateInEditor)
        {
            Debug.Log("GPS SIMULÉ (Editor)");

            latitude = editorLatitude;
            longitude = editorLongitude;
            gpsReady = true;

            if (positionText != null)
                positionText.text =
                    $"EDITOR GPS\nLat:{latitude:F4} Lon:{longitude:F4}";

            yield break;
        }
#endif

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
            SetText("GPS désactivé");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;

        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            SetText("Recherche GPS...");
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            SetText("GPS indisponible");
            yield break;
        }

        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;
        gpsReady = true;

        SetText($"Lat:{latitude:F4} Lon:{longitude:F4}");
    }

    void SetText(string msg)
    {
        if (positionText != null)
            positionText.text = msg;
    }
}