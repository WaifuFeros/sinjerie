using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[System.Serializable]
public class Stat
{
    public float BaseValue => _baseValue;

    [HideInInspector]
    public string name;

    public float Value
    {
        get
        {
            if (_isDirty)
            {
                _value = CalculateFinalValue();
                _isDirty = false;
            }
            return _value;
        }
    }

    [SerializeField] private float _baseValue;
    private bool _isDirty;
    private float _value;

    [SerializeField] private List<StatModifier> _statModifiers;
    public readonly ReadOnlyCollection<StatModifier> StatModifiers;

    public Stat()
    {
        _statModifiers = new List<StatModifier>();
        StatModifiers = _statModifiers.AsReadOnly();
    }

    public Stat(float baseValue) : this() => _baseValue = baseValue;

    public Stat(Stat stat) : this(stat.BaseValue)
    {
        foreach (var mod in stat.StatModifiers)
            AddModifier(mod);
    }

    public void AddModifier(StatModifier mod)
    {
        _isDirty = true;
        _statModifiers.Add(mod);
        _statModifiers.Sort(CompareModifierType);
    }

    private int CompareModifierType(StatModifier a, StatModifier b)
    {
        if (a.Order < b.Order)
            return -1;
        else if (a.Order > b.Order)
            return 1;
        return 0; // if (a.Order == b.Order)
    }

    public bool RemoveModifier(StatModifier mod)
    {
        if (_statModifiers.Remove(mod))
        {
            _isDirty = true;
            return true;
        }

        return false;
    }

    public bool RemoveAllModifiersFromSource(Object source)
    {
        bool didRemove = false;

        for (int i = _statModifiers.Count - 1; i >= 0; i--)
        {
            if (_statModifiers[i].Source == source)
            {
                _isDirty = true;
                didRemove = true;
                _statModifiers.RemoveAt(i);
            }
        }

        return didRemove;
    }

    private float CalculateFinalValue()
    {
        float finalValue = BaseValue;
        float sumPercentAdd = 0f;

        for (int i = 0; i < _statModifiers.Count; i++)
        {
            StatModifier mod = _statModifiers[i];

            switch (mod.Type)
            {
                case StatModType.Flat:
                    finalValue += mod.Value;
                    break;
                case StatModType.PercentAdd:
                    sumPercentAdd += (mod.Value / 100);
                    if (i + 1 >= _statModifiers.Count || _statModifiers[i + 1].Type != StatModType.PercentAdd)
                    {
                        finalValue *= 1 + sumPercentAdd;
                        sumPercentAdd = 0f;
                    }
                    break;
                case StatModType.PercentMultiply:
                    finalValue *= 1 + (mod.Value / 100);
                    break;
                default:
                    break;
            }
        }

        return Mathf.Round(finalValue);
    }

    public void ForceRecalculate() => _value = CalculateFinalValue();

    public static implicit operator float(Stat stat) => stat.Value;

    public override string ToString() => Value.ToString();
}
