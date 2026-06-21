using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static VisualEffectManager;


public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance { get; private set; }


    [Header("Combat Settings")]
    [SerializeField] private float enemyAttackDelay = 2f; // Délai avant que l'ennemi attaque

    [Header("Skip Turn Button")]
    [SerializeField] private UnityEngine.UI.Button skipTurnButton; // Bouton pour passer le tour

    [Header(" Effect Settings")]
    [SerializeField] private int _fireDuration;
    //[SerializeField] private int _freezeDuration;
    [SerializeField] private int _paralyzeDuration;
    [SerializeField] private int _wetDuration;
    [SerializeField] private int _addFireDuration;
    [SerializeField] private int _damageThunder;

    [Header("Animation References")]
    [SerializeField] private Transform _canvasParent;
    [SerializeField] private GameObject _itemAnimPrefab;
    [SerializeField] private Transform _enemyTransform;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _animationDuration = 0.8f;
    [SerializeField] private float _highlightSkipButtonSize = 1.2f;
    [SerializeField] private float _highlightSkipButtonTime = 0.2f;


    [Header("UI")]
    [SerializeField] public GameObject _playerhead;
    [SerializeField] private GameObject _ennemyhead;

    [Header("Data Storage")]
    [SerializeField] private SelectedCharacterData dataStorage;

    public Enemy currentEnemy;
    private System.Action onVictoryCallback;
    private System.Action onDefeatCallback;
    private bool combatActive = false;
    public bool isPlayerTurn = true;

    private RectTransform _skipButtonRectTransform;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        _skipButtonRectTransform = skipTurnButton.GetComponent<RectTransform>();
    }

    private void Start()
    {
        PlayerManager.Instance.OnStaminaUpdateEvent += CheckItemsAvailable;
    }

    private void OnDestroy()
    {
        DOTween.Kill(_skipButtonRectTransform);
        PlayerManager.Instance.OnStaminaUpdateEvent -= CheckItemsAvailable;
    }

    public void Initialize(Action onLoadCompleted)
    {
        // Configurer le bouton de passage de tour
        SetupSkipTurnButton();
        if (dataStorage.selectedCharacter != null)
        {
            PlayerManager.Instance.stats.Deck = dataStorage.selectedCharacter.startDeck;
            _playerhead.GetComponent<UnityEngine.UI.Image>().sprite = dataStorage.selectedCharacter.characterSprite;
        }
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
    public void StartCombat(System.Action onVictory, System.Action onDefeat, bool addDelayToItemSpawn = false)
    {
        onVictoryCallback = onVictory;
        onDefeatCallback = onDefeat;
        combatActive = true;
        isPlayerTurn = true;

        // Mettre la stamina au max
        PlayerManager.Instance.refillStamina();

        // Récupérer l'ennemi actuel
        if (RoomManager.Instance != null)
        {
            GameObject enemyObj = RoomManager.Instance.GetEnemy();
            if (enemyObj != null)
            {
                currentEnemy = enemyObj.GetComponent<Enemy>();
            }
        }

        MeteoCheck(); //verifier si il pleut pour appliquer l'effet de mouille au debut du combat

        // Ajout item dans l'inventaire du joueur
        StartCoroutine(AddItemsToPlayerInventory(addDelayToItemSpawn ? 0.25f : 0));

        SetupSkipTurnButtonInteractable(true);

    }

    private IEnumerator AddItemsToPlayerInventory(float delay)
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < PlayerManager.Instance.stats.nbStartItem; i++)
        {
            if (PlayerManager.Instance.stats.Deck.Length == 0 || !InventoryManager.Instance.HasEmptySlot())
                break;
            int randomIndex = UnityEngine.Random.Range(0, PlayerManager.Instance.stats.Deck.Length);
            ObjetSO obj = PlayerManager.Instance.stats.Deck[randomIndex];
            var deckList = new List<ObjetSO>(PlayerManager.Instance.stats.Deck);
            deckList.RemoveAt(randomIndex);
            PlayerManager.Instance.stats.Deck = deckList.ToArray();
            ItemManager.Instance.SpawnItem(obj);
        }
    }

    public void AttackPlayer(ObjetSO attack)
    {
        if (!combatActive)
            return;
        PlayerManager.Instance.TakeDamage(attack.objectEffect);
        CheckItemEffect(attack, true);
        CheckCombatEnd();
    }
    public void HealPlayer(ObjetSO healItem)
    {
        if (!combatActive)
            return;
        PlayerManager.Instance.Heal(healItem.objectEffect);
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
        StopHighlightSkipTurnButton();

        StartCoroutine(EndPlayerTurnSequence());
        return;

        MeteoCheck(); //verifier si il pleut pour appliquer l'effet de mouille a la fin du tour du joueur
        WeatherEffect.Instance.OnFire(false);
        WeatherEffect.Instance.OnWet(false);
        if (!combatActive || !isPlayerTurn)
        {
            return;
        }
        isPlayerTurn = false;
        SetupSkipTurnButtonInteractable(false);
        PlayerManager.Instance.refillStamina(); // Restaure la stamina du joueur à chaque tour passé
        // l'ennemis attack
        if(WeatherEffect.Instance.OnParalyze(false))
        {
            Debug.Log("aaaaaaaaaaaL'ennemi est paralysé et ne peut pas attaquer ce tour !");
            isPlayerTurn = true;
            SetupSkipTurnButtonInteractable(true);
            return;
        }
        else if (WeatherEffect.Instance.OnFreeze(false))
        {
            Debug.Log("L'ennemi est paralysé et ne peut pas attaquer ce tour !");
            isPlayerTurn = true;
            SetupSkipTurnButtonInteractable(true);
            return;
        }
        StartCoroutine(EnemyAttackSequence());
    }

    private IEnumerator EndPlayerTurnSequence()
    {
        MeteoCheck(); //verifier si il pleut pour appliquer l'effet de mouille a la fin du tour du joueur
        yield return WeatherEffect.Instance.OnFire(false);
        WeatherEffect.Instance.OnWet(false);
        if (!combatActive || !isPlayerTurn)
        {
            yield break;
        }
        isPlayerTurn = false;
        SetupSkipTurnButtonInteractable(false);
        PlayerManager.Instance.refillStamina(); // Restaure la stamina du joueur à chaque tour passé
        // l'ennemis attack
        if (WeatherEffect.Instance.OnParalyze(false))
        {
            Debug.Log("aaaaaaaaaaaL'ennemi est paralysé et ne peut pas attaquer ce tour !");
            yield return WeatherEffect.Instance.PlayParalyzeAnimation();
            isPlayerTurn = true;
            SetupSkipTurnButtonInteractable(true);
            yield break;
        }
        else if (WeatherEffect.Instance.OnFreeze(false))
        {
            Debug.Log("L'ennemi est gelé et ne peut pas attaquer ce tour !");
            yield return WeatherEffect.Instance.PlayFrozenAnimation();
            isPlayerTurn = true;
            SetupSkipTurnButtonInteractable(true);
            yield break;
        }

        yield return EnemyAttackSequence();
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
        else if (PlayerManager.Instance.IsDead())
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
            currentEnemy.EnemyStats.MaxHealth,
            currentEnemy.currentHealth,
            currentEnemy.currentStaminaMax
        );
        currentEnemy.currentStaminaMax = currentEnemy.EnemyStats.MaxStamina; // Reset stamina max pour l'effet snow
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
        yield return WeatherEffect.Instance.OnFire(true);
        WeatherEffect.Instance.OnWet(true);
        MeteoCheck(); //verifier si il pleut pour appliquer l'effet de mouille a la fin du tour de l'ennemi

        // pioche des items aléatoires à la fin du tour
        for (int i = 0; i < PlayerManager.Instance.stats.nbItemPerTurn; i++)
        {
            if (PlayerManager.Instance.stats.Deck.Length == 0 || !InventoryManager.Instance.HasEmptySlot())
            {
                print("############# tu n'as plus d'objet dans ton deck");
                break;
            }
            int randomIndex = UnityEngine.Random.Range(0, PlayerManager.Instance.stats.Deck.Length);
            ObjetSO obj = PlayerManager.Instance.stats.Deck[randomIndex];
            var deckList = new List<ObjetSO>(PlayerManager.Instance.stats.Deck);
            deckList.RemoveAt(randomIndex);
            PlayerManager.Instance.stats.Deck = deckList.ToArray();

            ItemManager.Instance.SpawnItem(obj);
        }

        if (!WeatherEffect.Instance.OnParalyze(true))
        {
            isPlayerTurn = true;
            SetupSkipTurnButtonInteractable(true);
        }
        else if (!WeatherEffect.Instance.OnFreeze(true))
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
        PlayerManager.Instance.FireCounter = 0;
        PlayerManager.Instance.FreezeCounter = 0;
        PlayerManager.Instance.WetCounter = 0;
        PlayerManager.Instance.ParalyzeCounter = 0;
        PlayerManager.Instance.UpdateAfflictionIcons();
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

    private void HighlightSkipTurnButton()
    {
        var startingScale = _skipButtonRectTransform.localScale;
        _skipButtonRectTransform.DOScale(_highlightSkipButtonSize, _highlightSkipButtonTime)
            .SetLoops(-1, LoopType.Yoyo)
            .SetTarget(_skipButtonRectTransform)
            .OnKill(() =>
            {
                _skipButtonRectTransform.localScale = startingScale;
            });
    }

    private void StopHighlightSkipTurnButton()
    {
        DOTween.Kill(_skipButtonRectTransform);
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
                //a reprendre la logique du feu car impression que prog deguelasse (il est 5h zebi j'ai plus de cerveau) mais normalement c'est fonctionnelle mais immonde
                case ObjetMaterialType.Fire:
                    // Applique l'effet de brulure
                    if (PlayerManager.Instance.WetCounter == 0)
                    {
                        PlayerManager.Instance.FireCounter += _fireDuration;
                        VisualEffectManager.Instance.AddEffect(_playerhead, ParticleEffectType.Fire);
                        if (PlayerManager.Instance.FreezeCounter > 0)
                            PlayerManager.Instance.FreezeCounter -= 1;
                    }
                    break;
                case ObjetMaterialType.Water:
                    VisualEffectManager.Instance.AddEffect(_playerhead, ParticleEffectType.Water);
                    if (PlayerManager.Instance.WetCounter == 0) 
                    { 
                        PlayerManager.Instance.WetCounter = _wetDuration; // Applique l'effet de mouille
                    }
                    else if (PlayerManager.Instance.WetCounter > 0)
                        PlayerManager.Instance.WetCounter += 1; // Applique l'effet de mouille +1 round..
                    break;
                case ObjetMaterialType.Ice:
                    //PlayerManager.Instance.FreezeCounter = _freezeDuration; // Applique l'effet de gel
                    PlayerManager.Instance.FreezeCounter += 1;
                    break;
                case ObjetMaterialType.PerfectIce:
                    PlayerManager.Instance.FreezeCounter += 2; // Applique l'effet de gel infini tant que pas soigné par un item ou autre
                    break;
                case ObjetMaterialType.Metal:
                    if (WeatherManager.Instance.effetMeteorologique == GameWeatherType.Thunderstorm)
                    {
                        PlayerManager.Instance.ParalyzeCounter = _paralyzeDuration; // Applique l'effet de paralysie pareil si pb compteur regarder le fix du feu.
                        WeatherEffect.Instance.Thunder(isPlayer, WeatherManager.Instance.effetMeteorologique == GameWeatherType.Thunderstorm, _damageThunder);
                    }
                    break;
                case ObjetMaterialType.Wood:
                    if (PlayerManager.Instance.FireCounter > 0)
                        PlayerManager.Instance.FireCounter += _addFireDuration; // Prolonge l'effet de brulure
                    break;
                case ObjetMaterialType.Electricity:
                    PlayerManager.Instance.ParalyzeCounter += 1;
                    break;
            }
            PlayerManager.Instance.UpdateAfflictionIcons();
        }
        else
        {
            switch (objet.objetMaterialType)
            {
                case ObjetMaterialType.Fire:
                    if (currentEnemy.WetCounter == 0)
                    {
                        currentEnemy.FireCounter = _fireDuration;
                        VisualEffectManager.Instance.AddEffect(_ennemyhead, ParticleEffectType.Fire);
                        ItemManager.Instance.UpdateAllReactions(WeatherManager.Instance.effetMeteorologique);
                        if (currentEnemy.FreezeCounter > 0)
                            currentEnemy.FreezeCounter -= 1;
                            ItemManager.Instance.UpdateAllReactions(WeatherManager.Instance.effetMeteorologique);


                    }
                    break;
                case ObjetMaterialType.Ice:
                    //currentEnemy.FreezeCounter = _freezeDuration; // Applique l'effet de gel
                    currentEnemy.FreezeCounter += 1;
                    ItemManager.Instance.UpdateAllReactions(WeatherManager.Instance.effetMeteorologique);
                    break;
                case ObjetMaterialType.Water:
                    VisualEffectManager.Instance.AddEffect(_ennemyhead, ParticleEffectType.Water);
                    if (currentEnemy.WetCounter == 0)
                    {
                        currentEnemy.WetCounter = _wetDuration; // Applique l'effet de mouille
                    }
                    else if (currentEnemy.WetCounter > 0)
                        currentEnemy.WetCounter += 1; // Applique l'effet de mouille +1 round..
                    break;
                case ObjetMaterialType.Metal:
                    if (WeatherManager.Instance.effetMeteorologique == GameWeatherType.Thunderstorm)
                    {
                        currentEnemy.ParalyzeCounter = _paralyzeDuration; // Applique l'effet de paralysie
                        ItemManager.Instance.UpdateAllReactions(WeatherManager.Instance.effetMeteorologique);
                        WeatherEffect.Instance.Thunder(isPlayer, WeatherManager.Instance.effetMeteorologique == GameWeatherType.Thunderstorm, _damageThunder);
                    }
                    break;
                case ObjetMaterialType.Wood:
                    if (currentEnemy.FireCounter > 0)
                        currentEnemy.FireCounter += _addFireDuration; // Prolonge l'effet de brulure
                        ItemManager.Instance.UpdateAllReactions(WeatherManager.Instance.effetMeteorologique);
                    break;
                case ObjetMaterialType.PerfectIce:
                    currentEnemy.FreezeCounter += 2;
                    ItemManager.Instance.UpdateAllReactions(WeatherManager.Instance.effetMeteorologique);
                    break;
                case ObjetMaterialType.Electricity:
                    currentEnemy.ParalyzeCounter += 1;
                    ItemManager.Instance.UpdateAllReactions(WeatherManager.Instance.effetMeteorologique);
                    break;
            }
            currentEnemy.UpdateAfflictionIcons();
        }
    }

    // Verifie les effets météorologiques au début et à la fin de chaque tour pour appliquer les effets correspondants
    public void MeteoCheck(bool isDebug = false)
    {
        if (WeatherManager.Instance.effetMeteorologique == GameWeatherType.Rain)
        {
            currentEnemy.WetCounter = _wetDuration;
            PlayerManager.Instance.WetCounter = _wetDuration;

            VisualEffectManager.Instance.AddEffect(_playerhead, ParticleEffectType.Water);
            VisualEffectManager.Instance.AddEffect(_ennemyhead, ParticleEffectType.Water);
            PlayerManager.Instance.UpdateAfflictionIcons();
            currentEnemy.UpdateAfflictionIcons();
        }
        else if (WeatherManager.Instance.effetMeteorologique == GameWeatherType.Snow)
        {
            WeatherEffect.Instance.isSnowing();
        }
    }

    private void CheckItemsAvailable()
    {
        var allItems = ItemManager.Instance.GetAllItems();

        if (allItems == null || allItems.Count == 0)
        {
            StopHighlightSkipTurnButton();
            return;
        }

        int lowestStaminaConsuption = int.MaxValue;
        foreach (var item in allItems)
        {
            if (item.itemData.objetWeight < lowestStaminaConsuption)
                lowestStaminaConsuption = item.itemData.objetWeight;
        }

        if (lowestStaminaConsuption > PlayerManager.Instance.stats.currentStamina)
            HighlightSkipTurnButton();
        else
            StopHighlightSkipTurnButton();
    }
}