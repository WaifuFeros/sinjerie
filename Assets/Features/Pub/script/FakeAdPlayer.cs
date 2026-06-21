using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class FakeAdPlayer : MonoBehaviour
{
    [Header("Unity Components")]
    public VideoPlayer player;
    public AudioSource audioSource;

    [Header("Ads")]
    public VideoClip[] ads;

    private void OnEnable()
    {
        // Quand le canvas s’ouvre → nouvelle pub
        PlayRandomAd();
    }

    private void OnDisable()
    {
        ClearAd();
    }

    IEnumerator Start()
    {
        yield return null;

        if (audioSource == null)
        {
            Debug.LogError("⚠️ Aucun AudioSource assigné !");
            yield break;
        }

        // CONFIG AUDIO
        player.controlledAudioTrackCount = 1;
        player.SetDirectAudioMute(0, true);
        player.audioOutputMode = VideoAudioOutputMode.AudioSource;
        player.SetTargetAudioSource(0, audioSource);
        player.EnableAudioTrack(0, true);
        player.SetDirectAudioVolume(0, 1f);
        player.SetDirectAudioMute(0, false);

        Debug.Log("=== AUDIO READY ===");
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

        // Reset complet du VideoPlayer
        player.prepareCompleted -= OnPrepared;
        player.Stop();
        player.clip = null;
        player.time = 0;
        player.frame = 0;

        player.clip = ads[index];

        Debug.Log("Clip audio tracks : " + player.clip.audioTrackCount);

        player.prepareCompleted += OnPrepared;
        player.Prepare();
    }

    private void OnPrepared(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnPrepared;

        vp.EnableAudioTrack(0, true);
        vp.SetDirectAudioMute(0, false);
        vp.SetDirectAudioVolume(0, 1f);

        Debug.Log("=== PREPARED ===");
        Debug.Log("AudioTrackCount : " + vp.audioTrackCount);
        Debug.Log("Muted : " + vp.GetDirectAudioMute(0));

        vp.Play();
    }


    public void ClearAd()
    {
        player.Stop();
        player.clip = null;

        // Efface la RenderTexture
        RenderTexture rt = (RenderTexture)player.targetTexture;
        if (rt != null)
        {
            RenderTexture.active = rt;
            GL.Clear(true, true, Color.black);
            RenderTexture.active = null;
        }

        Debug.Log("🧹 Pub nettoyée");
    }
}
