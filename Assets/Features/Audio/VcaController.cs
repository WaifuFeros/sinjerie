using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VcaController : MonoBehaviour
{
    public static VcaController Instance { get; private set; }
    public static event Action<string, float> OnVcaVolumeChanged;

    // Listes pour retenir le volume d'origine de chaque VCA lors d'un Fade
    private Dictionary<string, float> originalVolumes = new Dictionary<string, float>();
    private Dictionary<string, bool> isLoweredFlags = new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Permet de garder le son actif entre les scènes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Au démarrage du jeu, on force FMOD à appliquer les volumes sauvegardés
        // pour éviter que le son soit à 100% tant qu'on n'a pas ouvert le menu des options.
        InitializeVcaOnLaunch("Master");
        InitializeVcaOnLaunch("Music");
        // InitializeVcaOnLaunch("SFX"); // Décommente cette ligne si tu as un VCA pour les bruitages
    }

    private void InitializeVcaOnLaunch(string vcaName)
    {
        // On récupère la sauvegarde (1 par défaut si elle n'existe pas)
        float savedVolume = PlayerPrefs.GetFloat("VCA_" + vcaName, 1f);

        // On l'applique directement dans FMOD
        var vcaInstance = FMODUnity.RuntimeManager.GetVCA("vca:/" + vcaName);
        vcaInstance.setVolume(savedVolume);

        Debug.Log($"[AudioLaunch] Initialisation automatique de {vcaName} au volume : {savedVolume}");
    }

    /// <summary>
    /// Permet à un slider de récupérer le volume initial (sauvegardé ou FMOD) au démarrage
    /// </summary>
    public float GetSavedVolume(string vcaName)
    {
        if (PlayerPrefs.HasKey("VCA_" + vcaName))
        {
            return PlayerPrefs.GetFloat("VCA_" + vcaName);
        }

        var vca = FMODUnity.RuntimeManager.GetVCA("vca:/" + vcaName);
        vca.getVolume(out float fmodVolume);
        return fmodVolume;
    }

    /// <summary>
    /// Modifie le volume d'un VCA spécifique par son nom
    /// </summary>
    public void SetVolume(string vcaName, float volume)
    {
        bool isLowered = isLoweredFlags.ContainsKey(vcaName) && isLoweredFlags[vcaName];
        if (!isLowered) originalVolumes[vcaName] = volume;

        ApplyVolumeInternal(vcaName, volume, true);
    }

    private void ApplyVolumeInternal(string vcaName, float volume, bool saveToPrefs)
    {
        var vca = FMODUnity.RuntimeManager.GetVCA("vca:/" + vcaName);
        vca.setVolume(volume);

        bool isLowered = isLoweredFlags.ContainsKey(vcaName) && isLoweredFlags[vcaName];
        if (saveToPrefs && !isLowered)
        {
            PlayerPrefs.SetFloat("VCA_" + vcaName, volume);
            PlayerPrefs.Save();
        }

        OnVcaVolumeChanged?.Invoke(vcaName, volume);
    }

    // Le combat system appelle directement cette fonction globale (par défaut sur "Music")
    public void FadeLowerMusicVolume(float duration = 1.5f, float multiplier = 0.3f)
    {
        string vcaName = "Music";
        var vca = FMODUnity.RuntimeManager.GetVCA("vca:/" + vcaName);
        vca.getVolume(out float currentVolume);

        if (!isLoweredFlags.ContainsKey(vcaName) || !isLoweredFlags[vcaName])
        {
            originalVolumes[vcaName] = currentVolume;
            isLoweredFlags[vcaName] = true;
        }

        float targetVolume = originalVolumes[vcaName] * multiplier;

        DOTween.To(() => currentVolume, x => ApplyVolumeInternal(vcaName, x, false), targetVolume, duration)
            .SetEase(Ease.InOutSine);
    }

    public void FadeRestoreMusicVolume(float duration = 1.5f)
    {
        string vcaName = "Music";
        var vca = FMODUnity.RuntimeManager.GetVCA("vca:/" + vcaName);
        vca.getVolume(out float currentVolume);

        float targetVolume = originalVolumes.ContainsKey(vcaName) ? originalVolumes[vcaName] : 1f;

        DOTween.To(() => currentVolume, x => ApplyVolumeInternal(vcaName, x, false), targetVolume, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => isLoweredFlags[vcaName] = false);
    }

    /// <summary>
    /// Récupère le volume PHYSIQUE actuel du VCA directement depuis FMOD
    /// </summary>
    public float GetCurrentVolume(string vcaName)
    {
        var vcaInstance = FMODUnity.RuntimeManager.GetVCA("vca:/" + vcaName);
        vcaInstance.getVolume(out float currentVolume);
        return currentVolume;
    }
}