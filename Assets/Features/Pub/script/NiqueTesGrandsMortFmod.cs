using UnityEngine;
using FMODUnity;

public class CutWeatherMusic : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter emitter;

    // Appelé par ton bouton
    public void CutMusique()
    {
        // Méthode 1 : couper le volume
        emitter.EventInstance.setVolume(0f);

        // Méthode 2 (alternative) : arrêter la musique avec fade‑out
        // emitter.EventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        Debug.Log(" Musique FMOD coupée !");
    }

    public void OnResumeMusicButtonClicked()
    {
        emitter.EventInstance.setVolume(1f);
        Debug.Log(" Musique FMOD réactivée !");
    }
}
