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
        _videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    void Start()
    {
        // CORRECTION : On demande le volume RÉEL actuel à FMOD via le Singleton
        if (VcaController.Instance != null)
        {
            _currentMasterVolume = VcaController.Instance.GetCurrentVolume("Master");
            _currentCategoryVolume = VcaController.Instance.GetCurrentVolume(targetVcaName);
        }
        else // Sécurité au cas où le VcaController n'est pas encore là
        {
            _currentMasterVolume = PlayerPrefs.GetFloat("VCA_Master", 1f);
            _currentCategoryVolume = PlayerPrefs.GetFloat("VCA_" + targetVcaName, 1f);
        }
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

    private void OnVideoPrepared(VideoPlayer source)
    {
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

        if (_videoPlayer.isPrepared && _videoPlayer.controlledAudioTrackCount > 0)
        {
            _videoPlayer.SetDirectAudioVolume(0, finalVolume);
        }
    }
}