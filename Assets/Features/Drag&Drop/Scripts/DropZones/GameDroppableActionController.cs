using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GameDroppableActionController : GameDroppableZoneController<ItemBrain>
{
    public UnityEvent<ItemBrain> OnDropItem;

    [SerializeField]
    private PlayerStats _playerStats;

    protected override void onDropObject(GameDraggableObjectController draggable, PointerEventData eventData, bool isDropValid)
    {
        if (verifyDraggable(draggable, out var itemBrain) && _playerStats.removeStamina(itemBrain.itemData.objetWeight))
        {
            OnDropItem.Invoke(itemBrain);
            Destroy(draggable.gameObject);
        }
    }

    //protected override bool verifyDraggable(GameDraggableObjectController draggable)
    //{
    //    Debug.Log(draggable.GetType());
    //    return draggable is ItemBrain;
    //}
}
