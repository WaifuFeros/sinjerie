using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualEffectManager : MonoBehaviour
{
    public static VisualEffectManager Instance { get; private set; }

    public enum ParticleEffectType
    {
        Fire,
        Water,
        Paralyze,
        Freeze,
        Electric
    }

    [System.Serializable]
    internal struct ParticleEffectPair
    {
        public ParticleEffectType type;
        public GameObject prefab;
    }

    [Header("UI")]
    [SerializeField] private GameObject _uiRootContainer;

    [Header("Prefab")]
    [SerializeField] private ParticleEffectPair[] _particles;

    private Dictionary<(GameObject gameObject, ParticleEffectType type), VisualEffect> _visualEffectDictionary = new Dictionary<(GameObject, ParticleEffectType), VisualEffect>();

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public void AddEffect(GameObject targetGO, ParticleEffectType particleType)
    {
        if (targetGO == null)
            return;

        VisualEffect effect = null;
        if (_visualEffectDictionary.TryGetValue((targetGO, particleType), out effect))
        {
            effect.SetActive(true);
        }
        else
        {
            effect = SpawnVisualEffect(particleType, targetGO.transform);
            if (effect != null)
            {
                _visualEffectDictionary.Add((targetGO, particleType), effect);
                effect.SetActive(true);
            }
        }
    }

    public void RemoveEffect(GameObject targetGO, ParticleEffectType particleType)
    {
        if (targetGO == null)
            return;

        if (_visualEffectDictionary.TryGetValue((targetGO, particleType), out VisualEffect effect))
        {
            effect.SetActive(false);
        }
    }

    public void RemoveAllEffects(GameObject targetGO)
    {
        if (targetGO == null)
            return;

        foreach (var item in _visualEffectDictionary)
        {
            if (item.Key.gameObject == targetGO)
                item.Value.SetActive(false);
        }
    }

    public void TriggerBurst(GameObject targetGO, ParticleEffectType type)
    {
        if (targetGO == null)
            return;

        if (_visualEffectDictionary.TryGetValue((targetGO, type), out VisualEffect effect))
        {
            effect.TriggerBurst();
        }
        else
        {
            var newEffect = SpawnVisualEffect(type, targetGO.transform);
            if (newEffect != null)
            {
                _visualEffectDictionary.Add((targetGO, type), newEffect);
                newEffect.TriggerBurst();
            }
        }
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
        Image iconImage = CombatSystem.Instance.Enemy.enemyHead;
        Transform iconTransform = iconImage.transform;
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
        Image iconImage = CombatSystem.Instance.Enemy.enemyHead;
        Transform iconTransform = iconImage.transform;
        iconTransform.DOKill(true);
        if (iconImage != null) iconImage.DOKill(true);
        float duration = 0.5f;
        iconTransform.DOScale(new Vector3(0.8f, 1.2f, 1f), duration * 0.3f)
            .SetLoops(2, LoopType.Yoyo);
        Color originalColor = Color.white;
        iconImage.DOColor(new Color(0.5f, 0.9f, 0.5f), duration * 0.5f)
            .OnComplete(() =>
            {
                iconImage.DOColor(originalColor, duration * 0.5f);
            });
        
    }

    private VisualEffect SpawnVisualEffect(ParticleEffectType type, Transform parent)
    {
        foreach (var item in _particles)
        {
            if (item.type == type)
                return Instantiate(item.prefab, parent.position, parent.rotation, parent).GetComponent<VisualEffect>();
        }

        return null;
    }
}