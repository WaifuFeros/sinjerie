using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class LineTransform : MonoBehaviour
{
    public List<Transform> Points;

    [SerializeField] private LineRenderer _line;
    [SerializeField] private bool _forceWorldSpace = true;

    private void LateUpdate()
    {
        SetPosition();
    }

    private void SetPosition()
    {
        if (_line == null)
            return;

        for (int i = 0; i < Points.Count; i++)
        {
            if (Points[i] != null)
                _line.SetPosition(i, Points[i].position);
        }
    }

    private void OnValidate()
    {
        if (_line == null && !TryGetComponent(out _line))
            return;

        if (!_line.useWorldSpace)
        {
            if (_forceWorldSpace)
                _line.useWorldSpace = true;
            else
                Debug.LogWarning($"The {nameof(LineRenderer)} is set as local space. This component was designed to be used in world space and probably will not behave as intended.", this);
        }

        _line.positionCount = Points.Count;
    }
}
