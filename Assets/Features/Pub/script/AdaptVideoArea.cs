using UnityEngine;

public class AdaptVideoArea : MonoBehaviour
{
    public RectTransform videoArea;
    public float targetAspect = 16f / 9f;

    void Start()
    {
        Adapt();
    }

    void Adapt()
    {
        float screenW = Screen.width;
        float screenH = Screen.height;
        float screenAspect = screenW / screenH;

        if (screenAspect > targetAspect)
        {
            float height = screenH;
            float width = height * targetAspect;
            videoArea.sizeDelta = new Vector2(width, height);
        }
        else
        {
            float width = screenW;
            float height = width / targetAspect;
            videoArea.sizeDelta = new Vector2(width, height);
        }
    }

    public void RevertDeath()
    {
        for (int i = 0; i <= 3; i++)
        {
            ItemManager.Instance.SpawnRandomItem();
        }
        PlayerManager.Instance.Heal(PlayerManager.Instance.stats.maxHealth);
        CombatSystem.Instance.isPlayerTurn = true;
        CombatSystem.Instance.combatActive = true;
        CombatSystem.Instance.isDefeat = false;
        CombatSystem.Instance.isPlayed = false;
        CombatSystem.Instance.SetupSkipTurnButtonInteractable(true);
    }

}

