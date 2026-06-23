using UnityEngine;
using FMODUnity;

[RequireComponent(typeof(StudioEventEmitter))]
public class WeatherMusicManager : MonoBehaviour
{
    private StudioEventEmitter emitter;

    // On garde en mémoire la dernière météo connue pour ne mettre à jour FMOD que lors d'un changement
    private GameWeatherType previousWeather;
    private bool isInitialized = false;

    void Start()
    {
        emitter = GetComponent<StudioEventEmitter>();
    }

    void Update()
    {
        // On s'assure que le WeatherManager est bien chargé
        if (WeatherManager.Instance != null)
        {
            GameWeatherType currentWeather = WeatherManager.Instance.effetMeteorologique;

            // Au premier passage ou si la météo a changé, on met à jour FMOD
            if (!isInitialized || currentWeather != previousWeather)
            {
                UpdateFMODParameters(currentWeather);
                previousWeather = currentWeather;
                isInitialized = true;
            }
        }
    }

    // Traduit votre GameWeatherType en paramètres FMOD
    private void UpdateFMODParameters(GameWeatherType weatherType)
    {
        float volSun = 0f;
        float volSnow = 0f;
        float volStorm = 0f;

        // La logique de répartition selon vos règles
        switch (weatherType)
        {
            case GameWeatherType.ClearSky:
            case GameWeatherType.Mist:
                volSun = 1f; // ClearSky et Mist jouent la musique Soleil
                break;

            case GameWeatherType.Snow:
                volSnow = 1f; // Snow joue la musique Neige
                break;

            case GameWeatherType.Rain:
            case GameWeatherType.Thunderstorm:
                volStorm = 1f; // Rain et Thunderstorm jouent la musique Orage
                break;
        }

        // On envoie les valeurs à FMOD
        emitter.SetParameter("Sun", volSun);
        emitter.SetParameter("Snow", volSnow);
        emitter.SetParameter("Storm", volStorm);

        Debug.Log($"[FMOD] Changement de musique appliqué pour : {weatherType}");
    }
}