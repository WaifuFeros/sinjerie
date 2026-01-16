using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableManager : MonoBehaviour
{
    public static DraggableManager Instance { get; private set; }

    public Transform DraggingParent => _draggingParent;

    [SerializeField]
    private Transform _draggingParent;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public static bool IsValid()
    {
        return Instance != null;
    }
}
