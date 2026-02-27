using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class CombatSystem : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private RoomManager roomManager;
    [SerializeField] private float enemyAttackDelay = 2f; // Délai avant que l'ennemi attaque
    [SerializeField] private float shitDelay = 2f; // Fait caca tous les X secondes

    [Header("References")]
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private ItemManager _itemManager;

    [Header("Skip Turn Button")]
    [SerializeField] private UnityEngine.UI.Button skipTurnButton; // Bouton pour passer le tour


    [Header("Animation References")]
    [SerializeField] private Transform _canvasParent;
    [SerializeField] private GameObject _itemAnimPrefab;
    [SerializeField] private Transform _enemyTransform;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _animationDuration = 0.8f;

    private Enemy currentEnemy;
    private System.Action onVictoryCallback;
    private System.Action onDefeatCallback;
    private bool combatActive = false;
    private bool isPlayerTurn = true;

    public void Initialize()
    {
        // Configurer le bouton de passage de tour
        SetupSkipTurnButton();
    }

    ///<summary>
    ///Configure le bouton de passage de tour
    ///</summary>
    private void SetupSkipTurnButton()
    {
        if (skipTurnButton != null)
        {
            skipTurnButton.onClick.RemoveAllListeners();
            skipTurnButton.onClick.AddListener(() => OnSkipTurnButtonClicked());
        }
    }

    /// <summary>
    /// Démarre un combat
    /// </summary>
    public void StartCombat(System.Action onVictory, System.Action onDefeat)
    {
        onVictoryCallback = onVictory;
        onDefeatCallback = onDefeat;
        combatActive = true;
        isPlayerTurn = true;

        // Récupérer l'ennemi actuel
        if (roomManager != null)
        {
            GameObject enemyObj = roomManager.GetEnemy();
            if (enemyObj != null)
            {
                currentEnemy = enemyObj.GetComponent<Enemy>();
            }
        }

        // Ajout item dans l'inventaire du joueur
        for (int i = 0; i < _playerStats.stats.nbStartItem; i++)
        {
            if (_playerStats.stats.Deck.Length == 0 || !InventoryManager.Instance.HasEmptySlot())
                break;
            int randomIndex = UnityEngine.Random.Range(0, _playerStats.stats.Deck.Length);
            ObjetSO obj = _playerStats.stats.Deck[randomIndex];
            var deckList = new List<ObjetSO>(_playerStats.stats.Deck);
            deckList.RemoveAt(randomIndex);
            _playerStats.stats.Deck = deckList.ToArray();
            _itemManager.SpawnItem(obj);
        }

        SetupSkipTurnButtonInteractable(true);

    }

    public void AttackPlayer(ObjetSO attack)
    {
        if (!combatActive)
            return;
        _playerStats.TakeDamage(attack.objectEffect);
        CheckCombatEnd();
    }
    public void HealPlayer(ObjetSO healItem)
    {
        if (!combatActive)
            return;
        _playerStats.Heal(healItem.objectEffect);
        CheckCombatEnd();
    }

    public void AttackEnemy(ObjetSO attack)
    {
        if (!combatActive || currentEnemy == null)
            return;
        currentEnemy.TakeDamage(attack.objectEffect);
        CheckCombatEnd();
    }
    public void HealEnemy(ObjetSO healItem)
    {
        if (!combatActive || currentEnemy == null)
            return;
        currentEnemy.Heal(healItem.objectEffect);
        CheckCombatEnd();
    }

    /// <summary>
    /// Appelé quand le bouton de passage de tour est cliqué
    /// </summary>
    private void OnSkipTurnButtonClicked()
    {
        if (!combatActive || !isPlayerTurn)
        {
            return;
        }
        isPlayerTurn = false;
        SetupSkipTurnButtonInteractable(false);
        _playerStats.refillStamina(); // Restaure la stamina du joueur à chaque tour passé
        // l'ennemis attack
        StartCoroutine(EnemyAttackSequence());
    }

    /// <summary>
    /// Vérifie si le bouton de passage de tour doit être interactif
    /// </summary>
    private void SetupSkipTurnButtonInteractable(bool interactable)
    {
        if (skipTurnButton != null)
        {
            skipTurnButton.interactable = interactable;
        }
    }

    /// <summary>
    /// Vérifie les conditions de fin de combat après un délai
    /// </summary>
    private void CheckCombatEnd()
    {
        if (currentEnemy != null && currentEnemy.IsDead())
        {
            EndCombat(true);
        }
        else if (_playerStats.IsDead())
        {
            EndCombat(false);
        }
    }

    /// <summary>
    /// Séquence d'attaque de l'ennemi
    /// </summary>
    private IEnumerator EnemyAttackSequence()
    {
        if (!combatActive) yield break;

        ObjetSO[] chosenItems = currentEnemy.EnemyStats.behavior.ChooseItem(
            currentEnemy.EnemyStats.Items,
            currentEnemy.currentHealth,
            currentEnemy.EnemyStats.MaxStamina
        );

        foreach (ObjetSO item in chosenItems)
        {
            // Animation
            yield return StartCoroutine(AnimateItemThrow(item));
            // Appliquer l'effet de l'objet
            if (item.objectType == ObjetEffectType.Attack)
                AttackPlayer(item);
            else if (item.objectType == ObjetEffectType.Heal)
                HealEnemy(item);

            yield return new WaitForSeconds(0.4f); // Petit délai entre les attaques
        }


        CheckCombatEnd();

        isPlayerTurn = true;
        SetupSkipTurnButtonInteractable(true);

        // pioche des items aléatoires à la fin du tour
        for (int i = 0; i < _playerStats.stats.nbItemPerTurn; i++)
        {
            if (_playerStats.stats.Deck.Length == 0 || !InventoryManager.Instance.HasEmptySlot())
            {
                print("############# tu n'as plus d'objet dans ton deck");
                break;
            }
            int randomIndex = UnityEngine.Random.Range(0, _playerStats.stats.Deck.Length);
            ObjetSO obj = _playerStats.stats.Deck[randomIndex];
            var deckList = new List<ObjetSO>(_playerStats.stats.Deck);
            deckList.RemoveAt(randomIndex);
            _playerStats.stats.Deck = deckList.ToArray();

            _itemManager.SpawnItem(obj);
        }
    }
    private IEnumerator AnimateItemThrow(ObjetSO item)
    {
        GameObject projectile = Instantiate(_itemAnimPrefab, _canvasParent);
        projectile.transform.position = _enemyTransform.position;
        projectile.GetComponent<UnityEngine.UI.Image>().sprite = item.objetSprite;

        Vector3 startPos = _enemyTransform.position;
        Sequence throwSequence = DOTween.Sequence();

        if (item.objectType == ObjetEffectType.Heal)
        {
            Vector3 endPos = _enemyTransform.position;
            Vector3 midPoint = startPos + new Vector3(0, 350f, 0); // Petit décalage X pour la courbe
            Vector3[] path = new Vector3[] { startPos, midPoint, endPos };

            throwSequence.Append(projectile.transform.DOPath(path, _animationDuration, PathType.CatmullRom)
                .SetEase(Ease.InOutSine));
            throwSequence.Join(projectile.transform.DORotate(new Vector3(0, 0, -360), _animationDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.InOutSine));
        }
        else
        {
            Vector3 endPos = _playerTransform.position;
            Vector3 midPoint = (startPos + endPos) / 2 + new Vector3(0, 300f, 0);
            Vector3[] path = new Vector3[] { startPos, midPoint, endPos };

            throwSequence.Append(projectile.transform.DOPath(path, _animationDuration, PathType.CatmullRom)
                .SetEase(Ease.InQuad));

            throwSequence.Join(projectile.transform.DORotate(new Vector3(0, 0, -360), _animationDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear));
        }

        throwSequence.Join(projectile.transform.DOScale(2.5f, _animationDuration / 2).SetLoops(2, LoopType.Yoyo));

        yield return throwSequence.WaitForCompletion();
        Destroy(projectile);
    }
    private IEnumerator EndCombat(bool victory)
    {
        combatActive = false;
        isPlayerTurn = false;
        SetupSkipTurnButtonInteractable(false);

        yield return new WaitForSeconds(0.5f);

        // Vérifier si l'ennemi est mort

        if (victory)
        {
            Debug.Log("Victoire au combat!");
            onVictoryCallback?.Invoke();
        }
        else
        {
            Debug.Log("Défaite au combat!");
            onDefeatCallback?.Invoke();
        }
        yield break;
    }

    public bool IsCombatActive()
    {
        return combatActive;
    }

    public Enemy GetCurrentEnemy()
    {
        return currentEnemy;
    }
}