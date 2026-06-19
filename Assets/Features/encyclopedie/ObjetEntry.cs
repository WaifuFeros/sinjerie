using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ObjetEntry : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public Image background;

    private ObjetSO data;
    private EncyclopediaPanel encyclopedia;

    public void Setup(ObjetSO obj, EncyclopediaPanel panel)
    {
        data = obj;
        encyclopedia = panel;

        icon.sprite = obj.objetSprite;
        nameText.text = obj.objetName;

        // Couleur selon rareté
        background.color = RarityColor.GetColor(obj.Rarity);
    }

    public void OnClick()
    {
        DescriptionManager.Instance.DisplayDescription(data);
    }
}
