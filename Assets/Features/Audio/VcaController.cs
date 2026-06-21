using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VcaController : MonoBehaviour
{
    private FMOD.Studio.VCA vca;
    public string VcaName;

    private Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        vca = FMODUnity.RuntimeManager.GetVCA("vca:/" + VcaName);
        slider = GetComponent<Slider>();

        float volume;
        vca.getVolume(out volume);

        slider.SetValueWithoutNotify(volume);
    }

    public void SetVolume(float volume)
    {
        vca.setVolume(volume);
    }
    private float originalVolume = 1f;

    public void LowerMusicVolume(float multiplier = 0.3f)
    {
        vca.getVolume(out originalVolume);
        vca.setVolume(originalVolume * multiplier);
    }

    public void RestoreMusicVolume()
    {
        vca.setVolume(originalVolume);
    }
}
