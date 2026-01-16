using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDrop : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject item = eventData.pointerDrag;

        if (item != null)
        {
            Debug.Log("Item laché sur le slot !");
            ExecuteItemAction(item);
        }
    }

    private void ExecuteItemAction(GameObject item)
    {

        ObjetSO data = item.GetComponent<ItemBrain>().itemData;

        switch (data.objectType)
        {
            case ObjetEffectType.Heal:
                Debug.Log($"Soin appliqué : {data.objectEffect} PV");
                break;

            case ObjetEffectType.Attack:
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
        Destroy(item);
    }
}