using DG.Tweening;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static VisualEffectManager;


public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance { get; private set; }

    public bool isPlayerTurn { get; set; } = true;

    [Header("Audio")]
    [SerializeField] private EventReference hitSound;
    [SerializeField] private EventReference healSound;
    [SerializeField] private EventReference victorySound;
    [SerializeField] private EventReference defeatSound;

    [Header("Skip Turn Button")]
    [SerializeField] private UnityEngine.UI.Button skipTurnButton; // Bouton pour passer le tour

    #region Effect Settings

    [Header("Effect Settings")]
    [SerializeField] private int _initialFireDuration;
    [SerializeField] private int _subsequentFireDuration;
    [SerializeField] private int _woodOnFireAddDuration;
    [SerializeField] private int _iceOnFireLoseDuration;
    [Space]
    [SerializeField] private int _initialWetDuration;
    [SerializeField] private int _subsequentWetDuration;
    [Space]
    [SerializeField] private int _freezeDuration;
    [SerializeField] private int _perfectIceFreezeDuration;
    [SerializeField] private int _fireOnIceLoseDuration;
    public int freezeProcThreshold;
    [Space]
    [SerializeField] private int _metalParalyzeDuration;
    [SerializeField] private int _electricityParalyzeDuration;
    [SerializeField] private int _damageThunder;

    #endregion

    [Header("Animation References")]
    [SerializeField] private Transform _canvasParent;
    [SerializeField] private GameObject _itemAnimPrefab;
    [SerializeField] private float _animationDuration = 0.8f;
    [SerializeField] private float _highlightSkipButtonSize = 1.2f;
    [SerializeField] private float _highlightSkipButtonTime = 0.2f;

    [Header("Data Storage")]
    [SerializeField] private SelectedCharacterData dataStorage;

    [field: SerializeField] public Enemy Enemy { get; private set; }
    private System.Action onVictoryCallback;
    private System.Action onDefeatCallback;
    public bool combatActive = false;
    public bool isPlayed = false;
    public bool isDefeat;

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
        PlayerManager.Instance.OnAfflictionUpdateEvent += CheckPlayerFreeze;
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
            PlayerManager.Instance.playerHead.sprite = dataStorage.selectedCharacter.characterSprite;
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

        if (Enemy.EnemyStats.IsBoss)
            UIManager.Instance.ShowBossMessagePanel();

        //uunlock freeze item
        ItemManager.Instance.SetAllItemsActive(true);


        // Mettre la stamina au max
        PlayerManager.Instance.refillStamina();

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
            ItemManager.Instance.setActiveMetal();
        }
    }

    public void AttackPlayer(ObjetSO attack)
    {
        if (!combatActive)
            return;
        RuntimeManager.PlayOneShot(hitSound);
        PlayerManager.Instance.TakeDamage(attack.objectEffect);
        CheckItemEffect(attack, true);
        CheckCombatEnd();
    }
    public void HealPlayer(ObjetSO healItem)
    {
        if (!combatActive)
            return;
        RuntimeManager.PlayOneShot(healSound);
        PlayerManager.Instance.Heal(healItem.objectEffect);
        CheckItemEffect(healItem, true);
        CheckCombatEnd();
    }

    public void AttackEnemy(ObjetSO attack)
    {
        if (!combatActive || Enemy == null)
            return;
        RuntimeManager.PlayOneShot(hitSound);
        Enemy.TakeDamage(attack.objectEffect);
        CheckItemEffect(attack, false);
        CheckCombatEnd();
    }
    public void HealEnemy(ObjetSO healItem)
    {
        if (!combatActive || Enemy == null)
            return;

        RuntimeManager.PlayOneShot(healSound);
        Enemy.Heal(healItem.objectEffect);
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
    }

    private IEnumerator EndPlayerTurnSequence()
    {
        SetupSkipTurnButtonInteractable(false);
        isPlayerTurn = false;

        WeatherEffect.Instance.OnWet(false);
        MeteoCheck(); //verifier si il pleut pour appliquer l'effet de mouille a la fin du tour du joueur
        yield return WeatherEffect.Instance.OnFire(false);

        if (!combatActive)
            yield break;

        PlayerManager.Instance.refillStamina(); // Restaure la stamina du joueur à chaque tour passé

        bool enemyCanPlay = true;
        if (WeatherEffect.Instance.OnParalyze(false))
        {
            enemyCanPlay = false;
            Debug.Log("L'ennemi est paralysé et ne peut pas attaquer ce tour !");
            yield return WeatherEffect.Instance.PlayParalyzeAnimation(false);
        }
        
        if (WeatherEffect.Instance.CheckFreeze(false))
        {
            enemyCanPlay = false;
            Debug.Log("L'ennemi est gelé et ne peut pas attaquer ce tour !");
            yield return WeatherEffect.Instance.PlayFrozenAnimation(false);
            Enemy.FreezeCounter = 0;
        }

        if (enemyCanPlay)
        {
            yield return EnemyAttackSequence();
        }
        else
        {
            isPlayerTurn = true;
            SetupSkipTurnButtonInteractable(true);
        }
    }

    /// <summary>
    /// Vérifie si le bouton de passage de tour doit être interactif
    /// </summary>
    public void SetupSkipTurnButtonInteractable(bool interactable)
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
        if (isDefeat)
            return;

        if (Enemy != null && Enemy.IsDead())
            StartCoroutine(EndCombat(true));
        else if (PlayerManager.Instance.IsDead())
        {
            print("############# tu es mort");
            StartCoroutine(EndCombat(false));
        }
    }

    /// <summary>
    /// Séquence d'attaque de l'ennemi
    /// Reprendre toute la fonction pour respecter poids item cote ennemi. + faire un meilleur sys de fin de tour pour IA.
    /// </summary>
    private IEnumerator EnemyAttackSequence()
    {
        if (!combatActive) yield break;

        ObjetSO[] chosenItems = Enemy.EnemyStats.behavior.ChooseItem(
            Enemy.EnemyStats.Items,
            Enemy.EnemyStats.MaxHealth,
            Enemy.currentHealth,
            Enemy.currentStaminaMax
        );
        Enemy.currentStaminaMax = Enemy.EnemyStats.MaxStamina; // Reset stamina max pour l'effet snow
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

            if (Enemy.IsFrozen)
            {
                StartCoroutine(FreezeEnemyDuringHisTurn());
                yield break;
            }

            // Si le joueur est mort, arrête de le fracasser
            if (combatActive == false)
                yield break;
        }

        WeatherEffect.Instance.OnWet(true);
        MeteoCheck(); //verifier si il pleut pour appliquer l'effet de mouille a la fin du tour de l'ennemi
        yield return WeatherEffect.Instance.OnFire(true);

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

        bool playerCanPlay = true;
        if (WeatherEffect.Instance.OnParalyze(true))
        {
            Debug.Log("Le joueur est paralysé et ne peut pas attaquer ce tour !");
            playerCanPlay = false;
            yield return WeatherEffect.Instance.PlayParalyzeAnimation(true);
            SetupSkipTurnButtonInteractable(true);
        }
        if (WeatherEffect.Instance.CheckFreeze(true))
        {
            Debug.Log("Le joueur est gelé et ne peut pas attaquer ce tour !");
            playerCanPlay = false;
            yield return WeatherEffect.Instance.PlayFrozenAnimation(true);
            ItemManager.Instance.SetAllItemsActive(true);
            PlayerManager.Instance.FreezeCounter = 0;
        }

        if (playerCanPlay == false)
        {
            StartCoroutine(EnemyAttackSequence());
        }
        else
        {
            SetupSkipTurnButtonInteractable(true);
            isPlayerTurn = true;
        }

        if (TutorialManager.Instance.IsTutorial && PlayerManager.Instance.HasTakenDamage)
            TutorialPanelsManager.Instance.DisplayPanel(TutorialStep.ThrowOnPlayer);
    }

    private IEnumerator AnimateItemThrow(ObjetSO item)
    {
        GameObject projectile = Instantiate(_itemAnimPrefab, _canvasParent);
        projectile.transform.position = Enemy.enemyHead.transform.position;
        projectile.GetComponent<UnityEngine.UI.Image>().sprite = item.objetSprite;

        Vector3 startPos = Enemy.enemyHead.transform.position;
        Sequence throwSequence = DOTween.Sequence();

        if (item.objectType == ObjetEffectType.Heal)
        {
            Vector3 endPos = Enemy.enemyHead.transform.position;
            Vector3 midPoint = startPos + new Vector3(0, 350f, 0); // Petit décalage X pour la courbe
            Vector3[] path = new Vector3[] { startPos, midPoint, endPos };

            throwSequence.Append(projectile.transform.DOPath(path, _animationDuration, PathType.CatmullRom)
                .SetEase(Ease.InOutSine));
            throwSequence.Join(projectile.transform.DORotate(new Vector3(0, 0, -360), _animationDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.InOutSine));
        }
        else
        {
            Vector3 endPos = PlayerManager.Instance.playerHead.transform.position;
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
        Debug.Log("End Combat");
        PlayerManager.Instance.FireCounter = 0;
        PlayerManager.Instance.FreezeCounter = 0;
        PlayerManager.Instance.WetCounter = 0;
        PlayerManager.Instance.ParalyzeCounter = 0;
        combatActive = false;
        isPlayerTurn = false;
        SetupSkipTurnButtonInteractable(false);

        yield return new WaitForSeconds(0.5f);

        // Vérifier si l'ennemi est mort

        if (victory && !isPlayed)
        {
            Debug.Log("Victoire au combat!");
            VcaController.Instance.FadeLowerMusicVolume(2f, 0.2f);
            RuntimeManager.PlayOneShot(victorySound);
            onVictoryCallback?.Invoke();
            //isPlayed = true;
        }
        else if (!isPlayed)
        {
            isDefeat = true;
            Debug.Log("Défaite au combat!");
            RuntimeManager.PlayOneShot(defeatSound);
            VcaController.Instance.FadeLowerMusicVolume(2f, 0f);
            onDefeatCallback?.Invoke();
            isPlayed = true;
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

    private void CheckItemEffect(ObjetSO objet, bool isPlayer)
    {
        if (isPlayer)
        {
            switch (objet.objetMaterialType)
            {
                case ObjetMaterialType.Fire:
                    // Applique l'effet de brulure
                    if (PlayerManager.Instance.WetCounter == 0)
                    {
                        PlayerManager.Instance.FireCounter += PlayerManager.Instance.FireCounter > 0 ? _subsequentFireDuration : _initialFireDuration;
                        if (PlayerManager.Instance.FreezeCounter > 0)
                            PlayerManager.Instance.FreezeCounter -= _fireOnIceLoseDuration;
                    }
                    break;
                case ObjetMaterialType.Wood:
                    if (PlayerManager.Instance.FireCounter > 0)
                        PlayerManager.Instance.FireCounter += _woodOnFireAddDuration;
                    break;
                case ObjetMaterialType.Water:
                    PlayerManager.Instance.WetCounter += PlayerManager.Instance.WetCounter > 0 ? _subsequentWetDuration : _initialWetDuration;
                    PlayerManager.Instance.FireCounter = 0;
                    break;
                case ObjetMaterialType.Ice:
                    PlayerManager.Instance.FreezeCounter += _freezeDuration;
                    PlayerManager.Instance.FireCounter -= _iceOnFireLoseDuration;
                    break;
                case ObjetMaterialType.PerfectIce:
                    PlayerManager.Instance.FreezeCounter += _perfectIceFreezeDuration;
                    PlayerManager.Instance.FireCounter -= _iceOnFireLoseDuration;
                    break;
                case ObjetMaterialType.Metal:
                    if (WeatherManager.Instance.effetMeteorologique == GameWeatherType.Thunderstorm)
                    {
                        PlayerManager.Instance.ParalyzeCounter = _metalParalyzeDuration;
                        WeatherEffect.Instance.Thunder(isPlayer, WeatherManager.Instance.effetMeteorologique == GameWeatherType.Thunderstorm, _damageThunder);
                    }
                    break;
                case ObjetMaterialType.Electricity:
                    PlayerManager.Instance.ParalyzeCounter += _electricityParalyzeDuration;
                    break;
            }
        }
        else
        {
            switch (objet.objetMaterialType)
            {
                case ObjetMaterialType.Fire:
                    if (Enemy.WetCounter == 0)
                    {

                        Enemy.FireCounter += Enemy.FireCounter > 0 ? _subsequentFireDuration : _initialFireDuration;
                        if (Enemy.FreezeCounter > 0)
                            Enemy.FreezeCounter -= _fireOnIceLoseDuration;
                    }
                    break;
                case ObjetMaterialType.Wood:
                    if (Enemy.FireCounter > 0)
                        Enemy.FireCounter += _woodOnFireAddDuration;
                    break;
                case ObjetMaterialType.Water:
                    Enemy.WetCounter += Enemy.WetCounter > 0 ? _subsequentWetDuration : _initialWetDuration;
                    Enemy.FireCounter = 0;
                    break;
                case ObjetMaterialType.Ice:
                    Enemy.FreezeCounter += _freezeDuration;
                    Enemy.FireCounter -= _iceOnFireLoseDuration;
                    break;
                case ObjetMaterialType.PerfectIce:
                    Enemy.FreezeCounter += _perfectIceFreezeDuration;
                    Enemy.FireCounter -= _iceOnFireLoseDuration;
                    break;
                case ObjetMaterialType.Metal:
                    if (WeatherManager.Instance.effetMeteorologique == GameWeatherType.Thunderstorm)
                    {
                        Enemy.ParalyzeCounter = _metalParalyzeDuration;
                        WeatherEffect.Instance.Thunder(isPlayer, WeatherManager.Instance.effetMeteorologique == GameWeatherType.Thunderstorm, _damageThunder);
                    }
                    break;
                case ObjetMaterialType.Electricity:
                    Enemy.ParalyzeCounter += _electricityParalyzeDuration;
                    break;
            }
        }
    }

    // Verifie les effets météorologiques au début et à la fin de chaque tour pour appliquer les effets correspondants
    public void MeteoCheck(bool isDebug = false)
    {
        if (WeatherManager.Instance.effetMeteorologique == GameWeatherType.Rain)
        {
            Enemy.WetCounter = Mathf.Max(_initialWetDuration, Enemy.WetCounter);
            PlayerManager.Instance.WetCounter = Mathf.Max(_initialWetDuration, PlayerManager.Instance.WetCounter);
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
        {
            HighlightSkipTurnButton();

            if (TutorialManager.Instance.IsTutorial)
                TutorialPanelsManager.Instance.DisplayPanel(TutorialStep.SkipTurn);
        }
        else
            StopHighlightSkipTurnButton();
    }

    private void CheckPlayerFreeze()
    {
        if (isPlayerTurn && PlayerManager.Instance.IsFrozen)
        {
            StartCoroutine(SkipPlayerTurn());
        }
    }

    private IEnumerator SkipPlayerTurn()
    {
        ItemManager.Instance.SetAllItemsActive(false);

        VisualEffectManager.Instance.AddEffect(InventoryManager.Instance.itemsParent.gameObject, VisualEffectManager.ParticleEffectType.FreezeInventory);

        yield return new WaitForSeconds(1);

        HighlightSkipTurnButton();
    }

    private IEnumerator FreezeEnemyDuringHisTurn()
    {
        yield return new WaitForSeconds(1);

        isPlayerTurn = true;
        SetupSkipTurnButtonInteractable(true);
    }
}