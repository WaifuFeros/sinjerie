using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class UnityVideoLink : MonoBehaviour
{
    private VideoPlayer _videoPlayer;

    [Header("Configuration")]
    [SerializeField] private string targetVcaName = "Music";

    private float _currentMasterVolume = 1f;
    private float _currentCategoryVolume = 1f;

    void Awake()
    {
        _videoPlayer = GetComponent<VideoPlayer>();

        // ---> AJOUT : On écoute le moment où la vidéo est enfin chargée et prête
        _videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    void Start()
    {
        _currentMasterVolume = PlayerPrefs.GetFloat("VCA_Master", 1f);
        _currentCategoryVolume = PlayerPrefs.GetFloat("VCA_" + targetVcaName, 1f);

        // On n'appelle plus UpdateFinalVolume ici car la vidéo n'est pas encore prête
    }

    void OnEnable()
    {
        VcaController.OnVcaVolumeChanged += HandleVolumeChanged;
    }

    void OnDisable()
    {
        VcaController.OnVcaVolumeChanged -= HandleVolumeChanged;
        _videoPlayer.prepareCompleted -= OnVideoPrepared;
    }

    // ---> NOUVELLE FONCTION : Appelée automatiquement par Unity dès que la vidéo est prête
    private void OnVideoPrepared(VideoPlayer source)
    {
        Debug.Log("[VideoLink] La vidéo est prête ! Application du volume initial.");
        UpdateFinalVolume();
    }

    private void HandleVolumeChanged(string vcaName, float newVolume)
    {
        bool hasChanged = false;

        if (vcaName == "Master")
        {
            _currentMasterVolume = newVolume;
            hasChanged = true;
        }
        else if (vcaName == targetVcaName)
        {
            _currentCategoryVolume = newVolume;
            hasChanged = true;
        }

        if (hasChanged)
        {
            UpdateFinalVolume();
        }
    }

    private void UpdateFinalVolume()
    {
        float finalVolume = _currentMasterVolume * _currentCategoryVolume;

        // On vérifie si la vidéo est prête ET possède des pistes audio
        if (_videoPlayer.isPrepared && _videoPlayer.controlledAudioTrackCount > 0)
        {
            _videoPlayer.SetDirectAudioVolume(0, finalVolume);
            Debug.Log($"[VideoLink] Volume appliqué avec succès sur la vidéo : {finalVolume}");
        }
        else
        {
            Debug.LogWarning("[VideoLink] Impossible d'appliquer le volume : la vidéo n'est pas prête ou n'a pas de piste audio détectée.");
        }
    }
}