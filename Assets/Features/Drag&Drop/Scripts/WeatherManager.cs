using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;

public class WeatherManager : MonoBehaviour
{
    public GPSManager gpsManager;

    public TextMeshProUGUI cityText;
    public TextMeshProUGUI tempText;
    //public TextMeshProUGUI descText;
    //public TextMeshProUGUI coordText;
    public Image weatherIcon;

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

        cityText.text = data.name;
        tempText.text = $"{data.main.temp:F1} °C";
        string iconUrl = $"https://openweathermap.org/img/wn/{data.weather[0].icon}@2x.png";
        Debug.Log("URL icône météo : " + iconUrl);
        //coordText.text = $"{gpsManager.latitude:F4}, {gpsManager.longitude:F4}";

        StartCoroutine(LoadIcon(iconUrl));
    }

    IEnumerator LoadIcon(string url)
    {
        Debug.Log("Téléchargement de l'icône depuis : " + url);
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur icône : " + request.error);
                yield break;
            }

            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            Debug.Log("Texture téléchargée, taille : " + texture.width + "x" + texture.height);

            if (texture.width == 0 || texture.height == 0)
            {
                Debug.LogError("Texture vide ! Vérifie l'URL ou utilise un sprite local.");
                yield break;
            }

            Sprite sprite = Sprite.Create(texture,
                                          new Rect(0, 0, texture.width, texture.height),
                                          new Vector2(0.5f, 0.5f));
            weatherIcon.sprite = sprite;
            Debug.Log("Icône assignée !");
        }
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
    public string icon;
}