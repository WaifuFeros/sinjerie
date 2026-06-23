using UnityEngine;
using FMODUnity;
using DG.Tweening;

public class CutWeatherMusic : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter emitter;


    // Appelé par ton bouton
    public void CutMusique()
    {
        //// Méthode 1 : couper le volume
        ////gameplayMusicVCA.FadeLowerMusicVolume(2f, 0f);


        //emitter.EventInstance.setVolume(0f);

        //// Méthode 2 (alternative) : arrêter la musique avec fade‑out
        //// emitter.EventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        //Debug.Log(" Musique FMOD coupée !");
    }

    public void OnResumeMusicButtonClicked()
    {
        VcaController.Instance.FadeRestoreMusicVolume(1f);
    }
}
