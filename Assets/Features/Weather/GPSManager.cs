using UnityEngine;
using System.Collections;
using TMPro;

public class GPSManager : MonoBehaviour
{
    public float Latitude { get; private set; }
    public float Longitude { get; private set; }
    public bool GPSReady { get; private set; } = false;

    [Header("EDITOR DEBUG")]
    [SerializeField] private bool simulateInEditor = true;
    // Position de base : Angoulême
    [SerializeField] private float editorLatitude = 45.6484f;
    [SerializeField] private float editorLongitude = 0.1562f;

    [Space]
    [SerializeField] private TextMeshProUGUI positionText;

    IEnumerator Start()
    {
#if UNITY_EDITOR
        if (simulateInEditor)
        {
            Debug.Log("GPS SIMULÉ (Editor)");

            Latitude = editorLatitude;
            Longitude = editorLongitude;
            GPSReady = true;

            if (positionText != null)
                positionText.text =
                    $"EDITOR GPS\nLat:{Latitude:F4} Lon:{Longitude:F4}";

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

        Latitude = Input.location.lastData.latitude;
        Longitude = Input.location.lastData.longitude;
        GPSReady = true;

        SetText($"Lat:{Latitude:F4} Lon:{Longitude:F4}");
    }

    void SetText(string msg)
    {
        if (positionText != null)
            positionText.text = msg;
    }
}