using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class WeatherManager : MonoBehaviour
{
    public GPSManager gpsManager;

    public TextMeshProUGUI cityText;
    public TextMeshProUGUI tempText;
    public TextMeshProUGUI descText;
    [SerializeField, HideInInspector] public float temperature;
    [SerializeField, HideInInspector] public string effetMeteorologique;
    //public TextMeshProUGUI coordText;

    public string apiKey = "cab53e4ddd7d114609d442afdc97e4af";

    void Start()
    {
        Debug.Log("Start() appelé : lancement de la coroutine GetWeather");
        StartCoroutine(GetWeather());
    }

    IEnumerator GetWeather()
    {
        Debug.Log("Coroutine GetWeather démarrée, attente GPS...");
        yield return new WaitUntil(() => gpsManager != null && gpsManager.gpsReady);
        Debug.Log("GPS prêt ! Latitude: " + gpsManager.latitude + ", Longitude: " + gpsManager.longitude);

        string url =
            $"https://api.openweathermap.org/data/2.5/weather?" +
            $"lat={gpsManager.latitude}&lon={gpsManager.longitude}" +
            $"&appid={apiKey}&units=metric&lang=fr";

        UnityWebRequest request = UnityWebRequest.Get(url);
        Debug.Log("Envoi de la requête météo : " + url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            yield break;
        }

        Debug.Log("Requête météo réussie, récupération des données...");
        WeatherData data = JsonUtility.FromJson<WeatherData>(request.downloadHandler.text);
        temperature = data.main.temp; //save temperature for use in weather effect
        effetMeteorologique = data.weather[0].main; //save weather effect for use in weather effect system

        cityText.text = data.name;
        tempText.text = $"{data.main.temp:F1} °C";
        descText.text = data.weather[0].main;
        //coordText.text = $"{gpsManager.latitude:F4}, {gpsManager.longitude:F4}";
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
    public string main;
}