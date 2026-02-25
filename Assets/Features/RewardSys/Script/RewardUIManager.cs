using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RewardUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _ToggleButton;

    [SerializeField]
    private GameObject _ButtonValidate;

    [SerializeField]
    private Transform _Panel;

    [SerializeField]
    private TextMeshProUGUI _Text;

    [SerializeField]
    private int _NumberOfToggle;

    [SerializeField]
    private int _NumberOfRewardToChoose;

    public void AddToggle()
    {
        for (int i = 0; i < _NumberOfToggle; i++)
        {
            GameObject button = Instantiate(_ToggleButton, _Panel);
            print(button);
        }

        _Text.text = $"Choisissez {_NumberOfRewardToChoose} récompenses ";
    }

    public void ValidateChoice()
    {
        _Panel.gameObject.SetActive(false);
        for(int i = 0; i < _Panel.childCount; i++)
        {
            string Name = "Item(Clone)";
            if(Name == _Panel.GetChild(i).name)
            {
                print("test");
                Destroy(_Panel.GetChild(i).gameObject);
            }
        }
    }
}
