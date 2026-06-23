using System; // <-- AJOUT pour l'Action
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class VcaController : MonoBehaviour
{
    // ---> AJOUT : L'événement global que l'AudioSource va écouter
    public static event Action<string, float> OnVcaVolumeChanged;

    private FMOD.Studio.VCA vca;
    public string VcaName;

    private Slider slider;
    private float originalVolume = 1f;
    private bool isLowered = false;

    void Start()
    {
        vca = FMODUnity.RuntimeManager.GetVCA("vca:/" + VcaName);
        slider = GetComponent<Slider>();

        // On récupère le volume sauvegardé, ou le volume FMOD actuel par défaut
        float savedVolume = PlayerPrefs.GetFloat("VCA_" + VcaName, 1f);
        vca.getVolume(out float fmodVolume);

        float finalVolume = PlayerPrefs.HasKey("VCA_" + VcaName) ? savedVolume : fmodVolume;

        // Appliquer le volume initial
        SetVolume(finalVolume);

        if (slider != null)
        {
            slider.SetValueWithoutNotify(finalVolume);
        }
    }

    // ---> NOUVELLE MÉTHODE INTERNE : Centralise l'application du son et avertit Unity
    private void ApplyVolumeInternal(float volume, bool saveToPrefs)
    {
        vca.setVolume(volume);

        if (saveToPrefs && !isLowered)
        {
            PlayerPrefs.SetFloat("VCA_" + VcaName, volume);
            PlayerPrefs.Save(); // <--- AJOUTE CETTE LIGNE : Force la sauvegarde immédiate
        }

        OnVcaVolumeChanged?.Invoke(VcaName, volume);
    }

    public void SetVolume(float volume)
    {
        if (!isLowered) originalVolume = volume;
        ApplyVolumeInternal(volume, true);
    }

    public void FadeLowerMusicVolume(float duration = 1.5f, float multiplier = 0.3f)
    {
        vca.getVolume(out float currentVolume);

        if (!isLowered)
        {
            originalVolume = currentVolume;
            isLowered = true;
        }

        float targetVolume = originalVolume * multiplier;

        // Modifié pour passer par notre méthode interne (permet le fade de l'AudioSource Unity !)
        DOTween.To(() => currentVolume, x => ApplyVolumeInternal(x, false), targetVolume, duration)
            .SetEase(Ease.InOutSine);
    }

    public void FadeRestoreMusicVolume(float duration = 1.5f)
    {
        vca.getVolume(out float currentVolume);

        DOTween.To(() => currentVolume, x => ApplyVolumeInternal(x, false), originalVolume, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => isLowered = false);
        Debug.Log($"RESTORING");
    }
}