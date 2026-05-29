using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using DG.Tweening;


public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance { get; private set; }


    [Header("Combat Settings")]
    [SerializeField] private float enemyAttackDelay = 2f; // Délai avant que l'ennemi attaque

    [Header("Skip Turn Button")]
    [SerializeField] private UnityEngine.UI.Button skipTurnButton; // Bouton pour passer le tour

    [Header(" Effect Settings")]
    [SerializeField] private int _fireDuration;
    [SerializeField] private int _freezeDuration;
    [SerializeField] private int _paralyzeDuration;
    [SerializeField] private int _wetDuration;
    [SerializeField] private int _addFireDuration;
    [SerializeField] private int _playerIsFreeze;
    [SerializeField] private int _ennemiIsFreeze;
    [SerializeField] private int _damageThunder;
    [SerializeField] bool _isThunder = false;

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
    public bool isPlayerTurn = true;
    private RoomManager _roomManager;
    private ItemManager _itemManager;
    private PlayerManager _playerStats;
    private WeatherEffect _weatherEffect;
    [SerializeField] private WeatherManager weather;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public void Initialize(Action onLoadCompleted)
    {
        // Configurer le bouton de passage de tour
        _roomManager = RoomManager.Instance;
        _itemManager = ItemManager.Instance;
        _playerStats = PlayerManager.Instance;
        _weatherEffect = WeatherEffect.Instance;
        SetupSkipTurnButton();
        onLoadCompleted?.Invoke();
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

        // Mettre la stamina au max
        _playerStats.refillStamina();

        // Récupérer l'ennemi actuel
        if (_roomManager != null)
        {
            GameObject enemyObj = _roomManager.GetEnemy();
            if (enemyObj != null)
            {
                currentEnemy = enemyObj.GetComponent<Enemy>();
            }
        }

        MeteoCheck(); //verifier si il pleut pour appliquer l'effet de mouille au debut du combat

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
        CheckItemEffect(attack, true);
        CheckCombatEnd();
    }
    public void HealPlayer(ObjetSO healItem)
    {
        if (!combatActive)
            return;
        _playerStats.Heal(healItem.objectEffect);
        CheckItemEffect(healItem, true);
        CheckCombatEnd();
    }

    public void AttackEnemy(ObjetSO attack)
    {
        if (!combatActive || currentEnemy == null)
            return;
        currentEnemy.TakeDamage(attack.objectEffect);
        CheckItemEffect(attack, false);
        CheckCombatEnd();
    }
    public void HealEnemy(ObjetSO healItem)
    {
        if (!combatActive || currentEnemy == null)
            return;
        currentEnemy.Heal(healItem.objectEffect);
        CheckItemEffect(healItem, false);
        CheckCombatEnd();
    }

    /// <summary>
    /// Appelé quand le bouton de passage de tour est cliqué
    /// faire un meilleur sys de fin de tour pour joueur pour adapt effect.
    /// </summary>
    private void OnSkipTurnButtonClicked()
    {
        MeteoCheck(); //verifier si il pleut pour appliquer l'effet de mouille a la fin du tour du joueur
        _weatherEffect.OnFire(false);
        _weatherEffect.OnWet(false);
        if (!combatActive || !isPlayerTurn)
        {
            return;
        }
        isPlayerTurn = false;
        SetupSkipTurnButtonInteractable(false);
        _playerStats.refillStamina(); // Restaure la stamina du joueur à chaque tour passé
        // l'ennemis attack
        if(_weatherEffect.OnParalyze(false))
        {
            Debug.Log("L'ennemi est paralysé et ne peut pas attaquer ce tour !");
            isPlayerTurn = true;
            SetupSkipTurnButtonInteractable(true);
            return;
        }
        else if (_weatherEffect.OnFreeze(false,_ennemiIsFreeze))
        {
            Debug.Log("L'ennemi est paralysé et ne peut pas attaquer ce tour !");
            isPlayerTurn = true;
            SetupSkipTurnButtonInteractable(true);
            return;
        }
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
    public void CheckCombatEnd()
    {
        if (currentEnemy != null && currentEnemy.IsDead())
            StartCoroutine(EndCombat(true));
        else if (_playerStats.IsDead())
            StartCoroutine(EndCombat(false));
    }

    /// <summary>
    /// Séquence d'attaque de l'ennemi
    /// Reprendre toute la fonction pour respecter poids item cote ennemi. + faire un meilleur sys de fin de tour pour IA.
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
        _weatherEffect.OnFire(true);
        _weatherEffect.OnWet(true);
        MeteoCheck(); //verifier si il pleut pour appliquer l'effet de mouille a la fin du tour de l'ennemi

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

        if (!_weatherEffect.OnParalyze(true))
        {
            isPlayerTurn = true;
            SetupSkipTurnButtonInteractable(true);
        }
        else if (!_weatherEffect.OnFreeze(true,_playerIsFreeze))
        {
            isPlayerTurn = true;
            SetupSkipTurnButtonInteractable(true);
        }
        else
        {
            Debug.Log("Le joueur est paralysé et ne peut pas attaquer ce tour !");
            StartCoroutine(EnemyAttackSequence()); // Lancer le tour de l'ennemi
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
        _playerStats.FireCounter = 0;
        _playerStats.FreezeCounter = 0;
        _playerStats.WetCounter = 0;
        _playerStats.ParalyzeCounter = 0;
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


    private void CheckItemEffect(ObjetSO objet, bool isPlayer)
    {
        if (isPlayer)
        {
            switch (objet.objetMaterialType)
            {
                case ObjetMaterialType.Fire:
                    _playerStats.FireCounter = _fireDuration; // Applique l'effet de brulure
                    if (_playerIsFreeze > 0)
                    {
                        _playerIsFreeze -= 1; // Réduit le compteur de gel de l'ennemi
                    }
                    break;
                case ObjetMaterialType.Ice:
                    _playerStats.FreezeCounter = _freezeDuration; // Applique l'effet de gel
                    _playerIsFreeze += 1;
                    break;
                case ObjetMaterialType.Water:
                    _playerStats.WetCounter = _wetDuration; // Applique l'effet de mouille
                    break;
                case ObjetMaterialType.Metal:
                    _playerStats.ParalyzeCounter = _paralyzeDuration; // Applique l'effet de paralysie
                    _weatherEffect.Thunder(isPlayer, _isThunder, _damageThunder);
                    break;
                case ObjetMaterialType.Wood:
                    if (_playerStats.FireCounter > 0)
                        _playerStats.FireCounter += _addFireDuration; // Prolonge l'effet de brulure
                    break;
            }
        }
        else
        {
            switch (objet.objetMaterialType)
            {
                case ObjetMaterialType.Fire:
                    currentEnemy.FireCounter = _fireDuration; // Applique l'effet de brulure
                    if (_ennemiIsFreeze > 0)
                    { 
                        _ennemiIsFreeze -=1; // Réduit le compteur de gel de l'ennemi
                    }
                    break;
                case ObjetMaterialType.Ice:
                    currentEnemy.FreezeCounter = _freezeDuration; // Applique l'effet de gel
                    _ennemiIsFreeze += 1;
                    break;
                case ObjetMaterialType.Water:
                    currentEnemy.WetCounter = _wetDuration; // Applique l'effet de mouille
                    break;
                case ObjetMaterialType.Metal:
                    currentEnemy.ParalyzeCounter = _paralyzeDuration; // Applique l'effet de paralysie
                    _weatherEffect.Thunder(!isPlayer,_isThunder, _damageThunder);

                    break;
                case ObjetMaterialType.Wood:
                    if (currentEnemy.FireCounter > 0)
                        currentEnemy.FireCounter += _addFireDuration; // Prolonge l'effet de brulure
                    break;
            }
        }
    }

    // Verifie si il pleut pour appliquer l'effet de mouille au debut du combat et a la fin de chaque tour
    private void MeteoCheck()
    {
        return;
        if (weather.effetMeteorologique == "Rain")
        {
            currentEnemy.WetCounter = _wetDuration;
            _playerStats.WetCounter = _wetDuration;
        }
        else if (weather.effetMeteorologique == "Snow")
        {
            currentEnemy.FreezeCounter = _freezeDuration;
            _playerStats.FreezeCounter = _freezeDuration;
        }
        else if (weather.effetMeteorologique == "Mist" || weather.effetMeteorologique == "Drizzle")
        {

            //todo faire un sys qui cache les recompense
        }
        else if (weather.effetMeteorologique == "Thunderstorm")
        {
            _isThunder = true;
        }
    }
}