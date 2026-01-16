using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WeatherManager : MonoBehaviour
{
    public GPSManager gpsManager;
    public string apiKey = "cab53e4ddd7d114609d442afdc97e4af";

    void Start()
    {
        StartCoroutine(GetWeather());
    }

    IEnumerator GetWeather()
    {
        if (gpsManager == null)
        {
            Debug.LogError("gpsManager n'est pas assigné !");
            yield break;
        }

        yield return new WaitUntil(() => gpsManager.latitude != 0 && gpsManager.longitude != 0);

        string url = $"https://api.openweathermap.org/data/2.5/weather?" +
                     $"lat={gpsManager.latitude}&lon={gpsManager.longitude}" +
                     $"&appid={apiKey}&units=metric&lang=fr";

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Erreur météo : " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log("Réponse météo JSON : " + json);

            WeatherData data = JsonUtility.FromJson<WeatherData>(json);
            if (data != null && data.weather.Length > 0)
            {
                Debug.Log($"Ville : {data.name}, Température : {data.main.temp}°C, Description : {data.weather[0].description}");
            }
        }
    }
}

[System.Serializable]
public class WeatherData
{
    public Main main;
    public Weather[] weather;
    public string name;
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
