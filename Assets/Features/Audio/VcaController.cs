using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // <-- Ne pas oublier l'import de DOTween

public class VcaController : MonoBehaviour
{
    private FMOD.Studio.VCA vca;
    public string VcaName;

    private Slider slider;
    private float originalVolume = 1f;
    private bool isLowered = false; // Sécurité pour ne pas perdre le vrai volume de base

    void Start()
    {
        vca = FMODUnity.RuntimeManager.GetVCA("vca:/" + VcaName);
        slider = GetComponent<Slider>();

        vca.getVolume(out float volume);

        if (slider != null)
        {
            slider.SetValueWithoutNotify(volume);
        }
    }

    public void SetVolume(float volume)
    {
        vca.setVolume(volume);
        // Si la musique n'est pas atténuée, on met à jour le volume de référence
        if (!isLowered) originalVolume = volume;
    }

    /// <summary>
    /// Atténue progressivement le volume du VCA.
    /// </summary>
    public void FadeLowerMusicVolume(float duration = 1.5f, float multiplier = 0.3f)
    {
        vca.getVolume(out float currentVolume);

        // On sauvegarde le volume seulement la première fois qu'on le baisse
        if (!isLowered)
        {
            originalVolume = currentVolume;
            isLowered = true;
        }

        float targetVolume = originalVolume * multiplier;

        // Transition fluide du volume actuel vers le volume cible
        DOTween.To(() => currentVolume, x => vca.setVolume(x), targetVolume, duration)
            .SetEase(Ease.InOutSine);
    }

    /// <summary>
    /// Restaure progressivement le volume original du VCA.
    /// </summary>
    public void FadeRestoreMusicVolume(float duration = 1.5f)
    {
        vca.getVolume(out float currentVolume);

        DOTween.To(() => currentVolume, x => vca.setVolume(x), originalVolume, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => isLowered = false); // Réinitialise l'état à la fin du fade
    }
}