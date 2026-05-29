using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VisualEffectManager : MonoBehaviour
{
    public static VisualEffectManager Instance { get; private set; }

    public enum ParticleEffectType
    {
        Fire,
        Water,
    }

    [SerializeField] private GameObject _uiRootContainer;
    [SerializeField] private GameObject _fireParticlePrefab;
    [SerializeField] private GameObject _waterParticlePrefab;


    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public void AddEffect(GameObject targetGO, ParticleEffectType particleType)
    {
        if (targetGO == null)
            return;

        GameObject _particlePrefab = null;
        switch (particleType)
        {
            case ParticleEffectType.Fire:
                _particlePrefab = _fireParticlePrefab;
                break;
            case ParticleEffectType.Water:
                _particlePrefab = _waterParticlePrefab;
                break;
        }

        GameObject spawnedParticle = Instantiate(_particlePrefab, targetGO.transform.position, targetGO.transform.rotation);
        spawnedParticle.transform.SetParent(targetGO.transform);

        ParticleSystem ps = spawnedParticle.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
        }

    }

    public void RemoveEffect(GameObject particleGO)
    {
        if (particleGO == null) return;
        Destroy(particleGO);

    }

    public void ShakeUI(float duration = 0.5f, float strength = 15f, int vibrato = 10)
    {
        if (_uiRootContainer == null)
        {
            Debug.LogWarning("ShakeUI : Aucun GameObject parent assigné !");
            return;
        }

        Transform targetTransform = _uiRootContainer.transform;
        targetTransform.DOKill(true);

        targetTransform.DOShakePosition(duration, strength, vibrato, 90f, false, true)
            .OnComplete(() => {
                targetTransform.localPosition = Vector3.zero;
            });
    }
}