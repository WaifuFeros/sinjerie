using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


public class WeatherManager : MonoBehaviour
{
    public GPSManager gpsManager;

    public TextMeshProUGUI cityText;
    public TextMeshProUGUI tempText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI coordText;

    public string apiKey = "cab53e4ddd7d114609d442afdc97e4af";

    void Start()
    {
        StartCoroutine(GetWeather());
    }

    IEnumerator GetWeather()
    {
        yield return new WaitUntil(() => gpsManager != null && gpsManager.gpsReady);

        string url =
            $"https://api.openweathermap.org/data/2.5/weather?" +
            $"lat={gpsManager.latitude}&lon={gpsManager.longitude}" +
            $"&appid={apiKey}&units=metric&lang=fr";

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        WeatherData data =
            JsonUtility.FromJson<WeatherData>(request.downloadHandler.text);

        cityText.text = data.name;
        tempText.text = $"{data.main.temp:F1} °C";
        descText.text = data.weather[0].description;
        coordText.text = $"{gpsManager.latitude:F4}, {gpsManager.longitude:F4}";
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
