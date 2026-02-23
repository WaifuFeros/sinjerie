using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDrop : MonoBehaviour
{
    [SerializeField]
    private CombatSystem _combatSystem;

    public void ExecuteItemAction(ItemBrain item)
    {
        ObjetSO data = item.itemData;

        switch (data.objectType)
        {
            case ObjetEffectType.Heal:
                Debug.Log($"Soin appliqué : {data.objectEffect} PV");
                break;

            case ObjetEffectType.Attack:
                _combatSystem.AttackEnemy(item.itemData);
                Debug.Log($"Attaque effectuée : {data.objectEffect} Dégâts");
                break;

            case ObjetEffectType.Special:
                if (data.specialAction != null)
                {
                    data.specialAction.Execute();
                    Debug.Log("Action spéciale exécutée !");
                }
                else
                {
                    Debug.LogWarning("Aucune action spéciale assignée à cet objet.");
                }
                break;
        }
        Destroy(item.gameObject);
    }
}