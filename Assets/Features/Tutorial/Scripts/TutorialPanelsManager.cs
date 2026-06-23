using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanelsManager : MonoBehaviour
{
    public static TutorialPanelsManager Instance;

    [SerializeField] private TutorialPanel[] _panels;

    private HashSet<TutorialStep> _validatedSteps = new HashSet<TutorialStep>();

    private TutorialPanel _currentPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        Destroy(Instance);
    }

    public void DisplayPanel(TutorialStep step, bool validateStep = true)
    {
        if (step == TutorialStep.None || _validatedSteps.Contains(step))
            return;

        foreach (var item in _panels)
        {
            if (item.step == step)
            {
                _currentPanel = item;
                item.panel?.SetActive(true);

                if (validateStep)
                    _validatedSteps.Add(step);

                return;
            }
        }
    }

    public void HidePanel(TutorialStep step)
    {
        foreach (var item in _panels)
        {
            if (item.step == step)
            {
                item.panel?.SetActive(false);
                return;
            }
        }
    }

    public void HidePanel()
    {
        _currentPanel.panel?.SetActive(false);
    }

    [Serializable]
    internal struct TutorialPanel
    {
        public GameObject panel;
        public TutorialStep step;
    }
}

public enum TutorialStep
{
    None,
    ThrowOnEnemy,
    ThrowOnPlayer,
    SkipTurn
}
