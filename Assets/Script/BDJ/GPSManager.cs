using UnityEngine;
using System.Collections;

public class GPSManager : MonoBehaviour
{
    public float latitude;
    public float longitude;

    IEnumerator Start()
    {
#if UNITY_ANDROID
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.FineLocation))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.FineLocation);
        }
#endif

        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS désactivé par l'utilisateur");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.Log("GPS indisponible");
            yield break;
        }

        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;

        Debug.Log($"GPS OK : {latitude} / {longitude}");

        Input.location.Stop();
    }
}
