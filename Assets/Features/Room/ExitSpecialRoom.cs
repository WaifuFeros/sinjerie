using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitSpecialRoom : MonoBehaviour
{
    [SerializeField] private VcaController gameplayMusicVCA;

    public void ExitRoom()
    {
        GameLoopManager.Instance.ExitRoom();
        gameplayMusicVCA.FadeRestoreMusicVolume(2f);
    }
}
