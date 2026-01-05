using System.Collections;
using UnityEngine;
using System;

[System.Serializable]
public class Reward
{
    public int experience = 50;
    public int gold = 10;
    public bool hasItem = false;
    public string itemName = "";
}

public class RewardSystem : MonoBehaviour
{
    [Header("Reward Settings")]
    [SerializeField] private Reward baseReward;
    [SerializeField] private float victoryMultiplier = 1.5f;
    [SerializeField] private float defeatMultiplier = 0.3f;

    [Header("References")]
    [SerializeField] private PlayerStats playerStats;

    public void Initialize()
    {
        Debug.Log("RewardSystem initialisé");
    }

    public void GiveRewards(bool victory, int roomNumber, System.Action onComplete)
    {
        Reward reward = CalculateRewards(victory, roomNumber);

        StartCoroutine(DistributeRewards(reward, onComplete));
    }

    private Reward CalculateRewards(bool victory, int roomNumber)
    {
        Reward reward = new Reward();

        // Base reward
        reward.experience = baseReward.experience;
        reward.gold = baseReward.gold;

        // Multiplicateur selon victoire/défaite
        float multiplier = victory ? victoryMultiplier : defeatMultiplier;
        reward.experience = Mathf.RoundToInt(reward.experience * multiplier);
        reward.gold = Mathf.RoundToInt(reward.gold * multiplier);

        // Bonus selon le numéro de salle (difficulté)
        float roomBonus = 1f + (roomNumber * 0.1f);
        reward.experience = Mathf.RoundToInt(reward.experience * roomBonus);
        reward.gold = Mathf.RoundToInt(reward.gold * roomBonus);

        // Chance d'obtenir un item en cas de victoire
        if (victory && Random.Range(0f, 1f) < 0.3f)
        {
            reward.hasItem = true;
            reward.itemName = "Potion de soin";
        }

        return reward;
    }

    private IEnumerator DistributeRewards(Reward reward, System.Action onComplete)
    {
        Debug.Log($"Récompenses: {reward.experience} XP, {reward.gold} Or");

        // Donner l'expérience au joueur
        if (playerStats != null)
        {
            playerStats.AddExperience(reward.experience);
        }

        // Ici vous pouvez ajouter la gestion de l'or (inventaire, etc.)
        // InventoryManager.Instance.AddGold(reward.gold);

        // Donner l'item si présent
        if (reward.hasItem)
        {
            Debug.Log($"Item obtenu: {reward.itemName}");
            // InventoryManager.Instance.AddItem(reward.itemName);
        }

        yield return new WaitForSeconds(2f);

        onComplete?.Invoke();
    }
}
