using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherVisualManager : MonoBehaviour
{
    [Serializable]
    internal struct WeatherVisualPair
    {
        public GameWeatherType weather;
        public GameObject visual;
    }

    [SerializeField] private WeatherVisualPair[] _weathers;

    private GameObject _currentVisual;

    private void Start()
    {
        WeatherManager.Instance.OnWeatherChangedEvent += UpdateWeather;
    }

    private void OnDestroy()
    {
        WeatherManager.Instance.OnWeatherChangedEvent -= UpdateWeather;
    }

    private void UpdateWeather()
    {
        foreach (var weather in _weathers)
        {
            if (WeatherManager.Instance.effetMeteorologique == weather.weather)
            {
                _currentVisual?.SetActive(false);
                weather.visual.SetActive(true);
                _currentVisual = weather.visual;
                return;
            }
        }
    }
}
