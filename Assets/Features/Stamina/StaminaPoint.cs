using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StaminaPoint : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Image _outline;

    [SerializeField] private Sprite _staminaSpriteFull;
    [SerializeField] private Sprite _staminaSpriteEmpty;

    [SerializeField] private Color _isEnoughColor;
    [SerializeField] private Color _isNotEnoughColor;

    private void Start()
    {
        HideOutline();
    }

    private void OnDestroy()
    {
        DOTween.Kill(_outline);
    }

    public void SetStamina(bool isFull)
    {
        _icon.sprite = isFull ? _staminaSpriteFull : _staminaSpriteEmpty;
    }

    public void DisplayOutline(bool isEnough)
    {
        _outline.DOFade(1f, 1f)
            .OnStart(() =>
            {
                Color targetColor = isEnough ? _isEnoughColor : _isNotEnoughColor;
                targetColor.a = 1f;
                _outline.color = targetColor;
            })
            .SetLoops(-1, LoopType.Yoyo)
            .OnKill(() =>
            {
                HideOutline();
            });
    }

    public void HideOutline()
    {
        DOTween.Kill(_outline);
        _outline.DOFade(0f, 0f);
    }
}
