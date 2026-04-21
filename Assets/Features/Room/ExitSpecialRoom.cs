using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitSpecialRoom : MonoBehaviour
{
    public void ExitRoom()
    {
        GameLoopManager.Instance.ExitRoom();
    }
}
