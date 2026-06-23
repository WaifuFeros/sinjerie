using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

 public class DebugController : MonoBehaviour
{
    public GameObject debugPanel;
    public TMP_Dropdown weatherDropdown;
    public TMP_InputField temperatureInputField;

    private List<GameWeatherType> weatherList;

    private void Start()
    {
        debugPanel.SetActive(false);
    }

    public void OpenDebug()
    {
        InitWeatherDropdown();
        InitTemperature();
        debugPanel.SetActive(true);
    }

    public void InitWeatherDropdown()
    {
        weatherList = new List<GameWeatherType>();

        foreach (var weather in WeatherManager.Instance.conversionData)
        {
            if (weatherList.Contains(weather.asType) == false)
                weatherList.Add(weather.asType);
        }

        weatherDropdown.ClearOptions();
        weatherDropdown.AddOptions(weatherList.Select(x => x.ToString()).ToList());
    }

    public void InitTemperature()
    {
        temperatureInputField.SetTextWithoutNotify(WeatherManager.Instance.temperature.ToString());
    }

    public void SetWeather(int index)
    {
        WeatherManager.Instance.SetWeatherByType(weatherList[index]);
        CombatSystem.Instance?.MeteoCheck(true);
    }

    public void SetWeatherLock(bool value)
    {
        WeatherManager.Instance.LockWeather = value;
    }

    public void SetTemperature(string tempAsText)
    {
        if (float.TryParse(tempAsText, out float temp))
        {
            WeatherManager.Instance.SetTemperature(temp);
        }
    }
}
