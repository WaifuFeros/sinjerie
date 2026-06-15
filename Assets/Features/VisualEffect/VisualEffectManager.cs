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

    [Header("UI")]
    [SerializeField] private GameObject _uiRootContainer;
    [SerializeField] private GameObject _enemyIcon;


    [Header("Prefab")]
    [SerializeField] private GameObject _fireParticlePrefab;
    [SerializeField] private GameObject _waterParticlePrefab;



    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public void AddEffect(GameObject targetGO, ParticleEffectType particleType)
    {
        if (targetGO == null) return;

        // VERIFICATION : Si le targetGO a déjà un ParticleSystem dans ses enfants on le supprime
        ParticleSystem existingPS = targetGO.GetComponentInChildren<ParticleSystem>();
        if (existingPS != null)
        {
            existingPS.gameObject.SetActive(false);
            Destroy(existingPS.gameObject);
        }

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

        if (_particlePrefab == null) return;

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
    public void EnemyTakeDamage()
    {
        Transform iconTransform = _enemyIcon.transform;
        UnityEngine.UI.Image iconImage = _enemyIcon.GetComponent<UnityEngine.UI.Image>();
        iconTransform.DOKill(true);
        if (iconImage != null) iconImage.DOKill(true);
        float duration = 0.3f;
        iconTransform.DOShakePosition(duration, strength: 20f, vibrato: 15);
        iconTransform.DOScale(new Vector3(1.2f, 0.8f, 1f), duration * 0.3f) 
            .SetLoops(2, LoopType.Yoyo); 
        Color originalColor = Color.white;
        iconImage.DOColor(Color.red, duration * 0.2f)
            .OnComplete(() =>
            {
                iconImage.DOColor(originalColor, duration * 0.8f);
            });
        
    }

    public void EnemyHeal()
    {
        Transform iconTransform = _enemyIcon.transform;
        UnityEngine.UI.Image iconImage = _enemyIcon.GetComponent<UnityEngine.UI.Image>();
        iconTransform.DOKill(true);
        if (iconImage != null) iconImage.DOKill(true);
        float duration = 0.5f;
        iconTransform.DOScale(new Vector3(0.8f, 1.2f, 1f), duration * 0.3f)
            .SetLoops(2, LoopType.Yoyo);
        Color originalColor = Color.white;
        iconImage.DOColor(new Color(0.3f, 1f, 0.3f), duration * 0.5f)
            .OnComplete(() =>
            {
                iconImage.DOColor(originalColor, duration * 0.5f);
            });
        
    }
}