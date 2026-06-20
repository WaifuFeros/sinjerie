using UnityEngine;
using UnityEngine.Video;

public class FakeAdPlayer : MonoBehaviour
{
    public VideoPlayer player;
    public VideoClip[] ads;

    void Start()
    {
        PlayRandomAd();
    }

    public void PlayRandomAd()
    {
        if (ads.Length == 0)
        {
            Debug.LogWarning("Aucune vidéo de pub trouvée !");
            return;
        }

        int index = Random.Range(0, ads.Length);
        player.clip = ads[index];
        player.Play();
    }
}
