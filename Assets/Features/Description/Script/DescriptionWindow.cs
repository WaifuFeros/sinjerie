using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionWindow : MonoBehaviour
{
    [SerializeField] private RectTransform _descriptionWindow;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _materialTypeText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Image _itemImage;
    [SerializeField] private Image _rarityBorder;
    [SerializeField] private Image _effectImage;
    [SerializeField] private TextMeshProUGUI _effectText;
    [SerializeField] private TextMeshProUGUI _weightText;

    [Header("Animation")]
    [SerializeField] private float _hiddenScale;
    [SerializeField] private float _animationTime;

    private void Start()
    {
        // Hide
        _descriptionWindow.localScale = Vector3.one * _hiddenScale;
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    public void DisplayDescription(ObjetSO item)
    {
        if (item == null)
            return;

        _nameText.text = item.objetName;
        _descriptionText.text = item.objetDescription;
        _itemImage.sprite = item.objetSprite;

        if (item.objetMaterialType != ObjetMaterialType.None)
        {
            _materialTypeText.gameObject.SetActive(true);
            _materialTypeText.text = item.objetMaterialType.ToString();
        }
        else
        {
            _materialTypeText.gameObject.SetActive(false);
        }

        _rarityBorder.sprite = ItemManager.Instance.GetRaritySprite(item.Rarity);

        _effectImage.gameObject.SetActive(ItemManager.Instance.GetObjetTypeSprite(item.objectType, out Sprite result));
        _effectImage.sprite = result;

        _effectText.text = item.objectEffect.ToString();
        _weightText.text = item.objetWeight.ToString();

        StartAnimationFadeIn();
    }

    private void StartAnimationFadeIn()
    {
        DOTween.Kill(_descriptionWindow);
        var seq = DOTween.Sequence();

        seq.SetTarget(_descriptionWindow);
        seq.Append(_descriptionWindow.DOScale(1, _animationTime));
        seq.Join(_canvasGroup.DOFade(1, _animationTime));
        seq.OnComplete(() =>
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        });

        seq.Play();
    }

    public void HideDescription()
    {
        DOTween.Kill(_descriptionWindow);
        var seq = DOTween.Sequence();

        seq.SetTarget(_descriptionWindow);
        seq.Append(_descriptionWindow.DOScale(_hiddenScale, _animationTime));
        seq.Join(_canvasGroup.DOFade(0, _animationTime));
        seq.OnStart(() =>
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        });

        seq.Play();
    }
}
