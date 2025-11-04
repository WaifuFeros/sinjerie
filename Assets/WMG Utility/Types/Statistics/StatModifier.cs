using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatModifier
{
    public float Value => _value;
    public StatModType Type => _type;
    public int Order => _specialOrder ? _order : (int)_type;
    public Object Source => _source;

    [SerializeField] private float _value = 10;
    [SerializeField] private StatModType _type = StatModType.Flat;
    [SerializeField] private bool _specialOrder = false;
    [SerializeField] private int _order = 0;
    private Object _source;

    public StatModifier(float value, StatModType type, int order, Object source)
    {
        _value = value;
        _type = type;
        _order = order;
        _source = source;
    }

    public StatModifier(float value, StatModType type, int order) : this (value, type, order, null) { }

    public StatModifier(float value, StatModType type) : this (value, type, 0, null) { }

    public StatModifier(StatModifier mod, Object source) : this (mod.Value, mod.Type, 0, source) { }

    public void SetSource(Object source)
    {
        _source = source;
    }
}

public enum StatModType
{
    Flat = 0,
    PercentAdd = 100,
    PercentMultiply = 200
}