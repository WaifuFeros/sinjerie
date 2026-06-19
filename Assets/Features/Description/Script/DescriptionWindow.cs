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

    [Header("Asset")]
    [SerializeField] private Sprite _communSprite;
    [SerializeField] private Sprite _uncommonSprite;
    [SerializeField] private Sprite _rareSprite;
    [SerializeField] private Sprite _epicSprite;
    [SerializeField] private Sprite _lengendarySprite;
    [SerializeField] private Sprite _healSprite;
    [SerializeField] private Sprite _atkSprite;

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

        switch (item.Rarity)
        {
            case ObjetRarity.Common:
                _rarityBorder.sprite = _communSprite;
                break;
            case ObjetRarity.Uncommon:
                _rarityBorder.sprite = _uncommonSprite;
                break;
            case ObjetRarity.Rare:
                _rarityBorder.sprite = _rareSprite;
                break;
            case ObjetRarity.Epic:
                _rarityBorder.sprite = _epicSprite;
                break;
            case ObjetRarity.Legendary:
                _rarityBorder.sprite = _lengendarySprite;
                break;
        }

        _effectImage.gameObject.SetActive(true);

        if (item.objectType == ObjetEffectType.Heal)
            _effectImage.sprite = _healSprite;
        else if (item.objectType == ObjetEffectType.Attack)
            _effectImage.sprite = _atkSprite;
        else if (item.objectType == ObjetEffectType.Special)
            _effectImage.gameObject.SetActive(false);

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
