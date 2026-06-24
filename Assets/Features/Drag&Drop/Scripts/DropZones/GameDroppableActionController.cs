using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GameDroppableActionController : GameDroppableZoneController<ItemBrain>
{
    public UnityEvent<ItemBrain> OnDropItem;
    public bool AllowSpecial;


    protected override void onDropObject(GameDraggableObjectController draggable, PointerEventData eventData, bool isDropValid)
    {
        if (verifyDraggable(draggable, out var itemBrain) && CombatSystem.Instance.isPlayerTurn && PlayerManager.Instance.checkStamina(itemBrain.itemData.objetWeight) && CheckSpecial(draggable))
        {
            PlayerManager.Instance.removeStamina(itemBrain.itemData.objetWeight);
            OnDropItem.Invoke(itemBrain);
            Destroy(draggable.gameObject);
        }
    }

    private bool CheckSpecial(GameDraggableObjectController draggable)
    {
        if(AllowSpecial)
            return true;
        else
            return ((ItemBrain)draggable).ItemData.objectType != ObjetEffectType.Special;

    }
    //protected override bool verifyDraggable(GameDraggableObjectController draggable)
    //{
    //    Debug.Log(draggable.GetType());
    //    return draggable is ItemBrain;
    //}
}
