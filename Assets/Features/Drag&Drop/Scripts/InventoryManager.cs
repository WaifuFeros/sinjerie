using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private List<GameDroppableSlotController> _slots;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _slots = FindObjectsByType<GameDroppableSlotController>(FindObjectsSortMode.None).ToList();
            return;
        }

        Destroy(this);
    }

    public bool HasEmptySlot(int needed = 1)
    {
        int count = 0;
        foreach (var slot in _slots)
        {
            if (slot.IsEmpty)
            {
                count++;
                if (count >= needed)
                    return true;
            }
        }

        return false;
    }

    public GameDroppableSlotController GetEmptySlot()
    {
        List<GameDroppableSlotController> result = new List<GameDroppableSlotController>();
        foreach (var slot in _slots)
        {
            if (slot.IsEmpty)
                return slot;
        }

        return null;
    }

    public List<GameDroppableSlotController> GetEmptySlots(int needed)
    {
        if (!HasEmptySlot(needed))
            return null;

        List<GameDroppableSlotController> result = new List<GameDroppableSlotController>();
        foreach (var slot in _slots)
        {
            if (slot.IsEmpty)
            {
                result.Add(slot);
                if (result.Count >= needed)
                    return result;
            }
        }

        return result;
    }
}
