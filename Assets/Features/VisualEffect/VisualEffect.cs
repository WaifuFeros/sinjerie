using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VisualEffect : MonoBehaviour
{
    public bool IsActive { get; private set; }

    [SerializeField] private UnityEvent onDisplayVisualEffect;
    [SerializeField] private UnityEvent onHideVisualEffect;
    [SerializeField] private UnityEvent onTriggerBurst;

    public virtual void SetActive(bool setActive)
    {
        if (setActive == IsActive)
            return;

        if (setActive)
            onDisplayVisualEffect?.Invoke();
        else
            onHideVisualEffect?.Invoke();

        IsActive = setActive;
    }

    public virtual void TriggerBurst()
    {
        onTriggerBurst?.Invoke();
    }
}
