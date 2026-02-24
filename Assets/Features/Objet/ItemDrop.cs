using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDrop : MonoBehaviour
{
    [SerializeField]
    private CombatSystem _combatSystem;
    [SerializeField]
    private bool _isPlayer;

    public void ExecuteItemAction(ItemBrain item)
    {
        ObjetSO data = item.itemData;

        switch (data.objectType)
        {
            case ObjetEffectType.Heal:
                if (_isPlayer)
                    _combatSystem.HealPlayer(data);
                else
                    _combatSystem.HealEnemy(data);
                break;

            case ObjetEffectType.Attack:
                if (_isPlayer)
                    _combatSystem.AttackPlayer(data);
                else
                    _combatSystem.AttackEnemy(data);
                break;

            case ObjetEffectType.Special:
                if (_isPlayer)
                    data.specialAction.Execute();
                
                break;
        }
        Destroy(item.gameObject);
    }
}