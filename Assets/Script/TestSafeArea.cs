using TMPro;
using UnityEngine;

public class TestSafeArea : MonoBehaviour
{
    [SerializeField]
    private RectTransform _rectTransform;

    [SerializeField]
    private RectTransform _canvasRectTransform;

    [SerializeField]
    private TextMeshProUGUI _debugText;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _rectTransform.anchoredPosition = Screen.safeArea.position;
        _rectTransform.sizeDelta = Screen.safeArea.size * (1/_rectTransform.lossyScale.x);

        if (_debugText != null)
        {
            string text = string.Empty;

            text += $"Screen Size = {Screen.width}x{Screen.height}\n";
            text += $"Safe Area size = {Screen.safeArea.size}\n";
            text += $"SafeArea width & height = {Screen.safeArea.width}x{Screen.safeArea.height}\n";
            text += $"Canvas Scale = {_canvasRectTransform.lossyScale}\n";
            text += $"RectTransform Scale = {_rectTransform.lossyScale}\n";
            _debugText.text = text ;
        }
    }
}
