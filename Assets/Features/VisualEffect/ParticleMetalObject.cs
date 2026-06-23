using DG.Tweening;
using UnityEngine;
using static VisualEffectManager;

public class ParticleMetalObject : MonoBehaviour
{


    void OnEnable()
    {
        StartParticule();
    }

    void OnDisable()
    {
        StopParticle();
    }

    public void StartParticule()
    {
        VisualEffectManager.Instance.AddEffect(gameObject,ParticleEffectType.Electric);
    }

    public void StopParticle()
    {
        VisualEffectManager.Instance.RemoveEffect(gameObject,ParticleEffectType.Electric);
    }
}

