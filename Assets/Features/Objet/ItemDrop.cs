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
                print("hey");
                if (_isPlayer)
                {
                    print("1");
                    _combatSystem.AttackPlayer(data);
                }
                else
                {
                    print("2");
                    _combatSystem.AttackEnemy(data);
                }

                Debug.Log($"Attaque effectuée : {data.objectEffect} Dégâts");
                break;

            case ObjetEffectType.Special:
                if (_isPlayer)
                    data.specialAction.Execute();
                
                break;
        }
        Destroy(item.gameObject);
    }
}