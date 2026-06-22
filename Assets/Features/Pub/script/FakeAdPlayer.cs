using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class FakeAdPlayer : MonoBehaviour
{
    [Header("Unity Components")]
    public VideoPlayer player;
    public AudioSource audioSource;

    [Header("UI")]
    public GameObject closeButton;

    [Header("Ads")]
    public VideoClip[] ads;

    private void OnEnable()
    {
        if (closeButton != null)
            closeButton.SetActive(false);

        PlayRandomAd();
    }

    private void OnDisable()
    {
        ClearAd();
    }

    IEnumerator Start()
    {
        yield return null;

        // CONFIG AUDIO
        player.controlledAudioTrackCount = 1;
        player.SetDirectAudioMute(0, true);
        player.audioOutputMode = VideoAudioOutputMode.AudioSource;
        player.SetTargetAudioSource(0, audioSource);
        player.EnableAudioTrack(0, true);
        player.SetDirectAudioVolume(0, 1f);
        player.SetDirectAudioMute(0, false);

        // Événement de fin de vidéo
        player.loopPointReached += OnVideoFinished;
    }

    public void PlayRandomAd()
    {
        if (ads.Length == 0)
        {
            Debug.LogWarning("Aucune pub trouvée !");
            return;
        }

        int index = Random.Range(0, ads.Length);
        Debug.Log("Pub choisie : " + ads[index].name);

        // Reset complet
        player.prepareCompleted -= OnPrepared;
        player.Stop();
        player.clip = null;
        player.time = 0;
        player.frame = 0;

        player.clip = ads[index];

        player.prepareCompleted += OnPrepared;
        player.Prepare();
    }
    private void OnPrepared(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnPrepared;

        vp.EnableAudioTrack(0, true);
        vp.SetDirectAudioMute(0, false);
        vp.SetDirectAudioVolume(0, 1f);

        // Cache le bouton au cas où
        if (closeButton != null)
            closeButton.SetActive(false);

        vp.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Pub terminée !");

        if (closeButton != null)
            closeButton.SetActive(true);  
    }

    public void ClearAd()
    {
        player.Stop();
        player.clip = null;

        RenderTexture rt = (RenderTexture)player.targetTexture;
        if (rt != null)
        {
            RenderTexture.active = rt;
            GL.Clear(true, true, Color.black);
            RenderTexture.active = null;
        }

        if (closeButton != null)
            closeButton.SetActive(false);
    }
}
