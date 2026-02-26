using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleAssignation : MonoBehaviour
{
    public ObjetSO _Item { get; private set; }
    public void Initialized(ObjetSO itemsInToggle)
    {
        _Item = itemsInToggle;
    }
}
