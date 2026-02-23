using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GameAttackDropZone : GameDroppableZoneController<ItemBrain>
{

    [SerializeField]
    private PlayerStats _playerStats;

    public UnityEvent<ItemBrain> OnDropItem;

    public string TestMessage;

    protected override void OnDropObject(ItemBrain draggable, PointerEventData eventData, bool isDropValid)
    {
        Debug.Log(draggable.GetType());
        base.OnDropObject(draggable, eventData, isDropValid);
        if (isDropValid && _playerStats.removeStamina(draggable.itemData.objetWeight))
        {
            OnDropItem.Invoke(draggable);
        }
        
        
    }

    protected override bool verifyDraggable(GameDraggableObjectController draggable)
    {
        Debug.Log(draggable.GetType());
        return draggable is ItemBrain;
    }
}
