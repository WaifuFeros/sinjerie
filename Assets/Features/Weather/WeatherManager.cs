using System;
using System.Collections;
using TMPro;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.Networking;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance { get; private set; }
    public float temperature { get; private set; }
    public GameWeatherType effetMeteorologique { get; private set; }
    public bool LockWeather { get; set; } = false;

    [SerializeField] private GPSManager gpsManager;
    [SerializeField, Min(0)] private float weatherUpdateInterval;

    public WeatherConversionData[] conversionData;

    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI cityText;
    [SerializeField] private TextMeshProUGUI tempText;
    [SerializeField] private TextMeshProUGUI descText;

    [SerializeField, ReadOnlyField] private string apiKey = "cab53e4ddd7d114609d442afdc97e4af";

    private DateTime _lastSaveTime;

    private readonly string TIME_STAMP_SAVE_KEY = "weather_time_stamp_save_key";

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(this); }
    }

    void Start()
    {
        LoadTimeStamp();
        //Debug.Log("Start() appelé : lancement de la coroutine GetWeather");
        //StartCoroutine(GetWeather());
    }

    public void UpdateWeather(Action onLoadComplete)
    {
        if (LockWeather)
            return;

        if (DateTime.UtcNow > _lastSaveTime.AddSeconds(weatherUpdateInterval))
            StartCoroutine(GetWeatherRequest(onLoadComplete));
        else
            onLoadComplete?.Invoke();
    }

    private IEnumerator GetWeatherRequest(Action onLoadComplete)
    {
        if (LockWeather)
            yield break;

        Debug.Log("Coroutine GetWeather démarrée, attente GPS...");
        yield return new WaitUntil(() => gpsManager != null && gpsManager.GPSReady);
        Debug.Log("GPS prêt ! Latitude: " + gpsManager.Latitude + ", Longitude: " + gpsManager.Longitude);

        string url =
            $"https://api.openweathermap.org/data/2.5/weather?" +
            $"lat={gpsManager.Latitude}&lon={gpsManager.Longitude}" +
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
        //effetMeteorologique = data.weather[0].main; //save weather effect for use in weather effect system
        effetMeteorologique = convertWeatherStateToGameWeather(data.weather[0].main);

        DisplayDebug(data.name, data.main.temp, data.weather[0].main);
        ItemManager.Instance.UpdateAllReactions(effetMeteorologique); // to move
        //coordText.text = $"{gpsManager.latitude:F4}, {gpsManager.longitude:F4}";

        SaveTimeStamp();
        onLoadComplete?.Invoke();
    }

    private GameWeatherType convertWeatherStateToGameWeather(string stateString)
    {
        GameWeatherType weatherType = GameWeatherType.ClearSky;
        for (int i = 0; i < conversionData.Length; i++)
        {
            if (conversionData[i].weatherMain == stateString)
            {
                weatherType = conversionData[i].asType;
                break;
            }
            else if (conversionData[i].isDefault)
            {
                weatherType = conversionData[i].asType;
            }
        }

        return weatherType;
    }

    #region Debug

    private void DisplayDebug(string city, float temperature, string description)
    {
        cityText?.SetText(city);
        tempText?.SetText($"{temperature:F1} °C");
        descText?.SetText(description);
    }

    public void SetWeatherByType(GameWeatherType type)
    {
        foreach (var item in conversionData)
        {
            if (item.asType == type)
            {
                effetMeteorologique = item.asType;
                descText.text = effetMeteorologique.ToString();
                return;
            }
        }
    }

    public void SetTemperature(float temperature)
    {
        this.temperature = temperature;
        tempText.text = $"{this.temperature:F1} °C";
    }

    #endregion

    #region Save time stamp logic

    [Serializable]
    internal struct WeatherSaveTimeStampFormat
    {
        public string dateTime;
        public float temperature;
        public GameWeatherType weatherType;

        public WeatherSaveTimeStampFormat(DateTime time, float temp, GameWeatherType type)
        {
            dateTime = time.ToString("o");
            temperature = temp;
            weatherType = type;
        }
    }

    private void SaveTimeStamp()
    {
        _lastSaveTime = DateTime.UtcNow;
        string json = JsonUtility.ToJson(new WeatherSaveTimeStampFormat(_lastSaveTime, temperature, effetMeteorologique));
        PlayerPrefs.SetString(TIME_STAMP_SAVE_KEY, JsonUtility.ToJson(new WeatherSaveTimeStampFormat(_lastSaveTime, temperature, effetMeteorologique)));
        PlayerPrefs.Save();
    }

    private void LoadTimeStamp()
    {
        if (PlayerPrefs.HasKey(TIME_STAMP_SAVE_KEY))
        {
            WeatherSaveTimeStampFormat save = JsonUtility.FromJson<WeatherSaveTimeStampFormat>(PlayerPrefs.GetString(TIME_STAMP_SAVE_KEY));
            _lastSaveTime = DateTime.Parse(save.dateTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
            temperature = save.temperature;
            effetMeteorologique = save.weatherType;
        }
        else
        {
            UpdateWeather(null);
        }
    }

    #endregion

    #region Api Request classes

    [System.Serializable]
    internal class WeatherData
    {
        public string name;
        public Main main;
        public Weather[] weather;
    }

    [System.Serializable]
    internal class Main
    {
        public float temp;
    }

    [System.Serializable]
    internal class Weather
    {
        public string main;
    }

    [Serializable]
    public class WeatherConversionData
    {
        public string weatherMain;
        public GameWeatherType asType;
        public bool isDefault;
    }

    #endregion
}

public enum GameWeatherType
        {
            ClearSky,
            Rain,
            Mist,
            Thunderstorm,
            Snow
        }