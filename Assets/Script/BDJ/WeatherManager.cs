using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class WeatherManager : MonoBehaviour
{
    public GPSManager gpsManager;
    public TextMeshProUGUI positionText;
    public string apiKey = "cab53e4ddd7d114609d442afdc97e4af";

    void Start()
    {
        StartCoroutine(GetWeather());
    }

    IEnumerator GetWeather()
    {
        // Sécurité anti NullReference
        if (gpsManager == null || positionText == null)
        {
            Debug.LogError("Références manquantes");
            yield break;
        }

        // Attente GPS
        yield return new WaitUntil(() => gpsManager.gpsReady);

        string url =
            $"https://api.openweathermap.org/data/2.5/weather?" +
            $"lat={gpsManager.latitude}&lon={gpsManager.longitude}" +
            $"&appid={apiKey}&units=metric&lang=fr";

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            positionText.text = "Erreur météo";
            Debug.LogError(request.error);
            yield break;
        }

        WeatherData data =
            JsonUtility.FromJson<WeatherData>(request.downloadHandler.text);

        positionText.text =
            $"{data.name}\n" +
            $"{data.main.temp:F1} °C\n" +
            $"{data.weather[0].description}\n\n";
            //$"Lat: {gpsManager.latitude:F5}\n" +
            //$"Lon: {gpsManager.longitude:F5}";
    }
}

[System.Serializable]
public class WeatherData
{
    public string name;
    public Main main;
    public Weather[] weather;
}

[System.Serializable]
public class Main
{
    public float temp;
}

[System.Serializable]
public class Weather
{
    public string description;
}

