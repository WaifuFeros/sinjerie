using UnityEngine;

public class WeatherEffect : MonoBehaviour
{
    [Header("Settings Effect")]
    [SerializeField] private float fireResistance;
    [SerializeField] private int fireDamage;

    private Enemy enemy;
    private Main main; //Class Main de Weather Manager
    public void OnFire()
    {
        if(main.temp >= fireResistance || enemy.FireCounter > 0)
        {
            enemy.TakeDamage(fireDamage);
            enemy.FireCounter--;
        }
    }
}
