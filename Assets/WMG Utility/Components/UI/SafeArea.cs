using UnityEngine;

namespace WMG.Utilities
{
    public class TestSafeArea : MonoBehaviour
    {
        private void Awake()
        {
            RectTransform rectTransform = transform as RectTransform;
            rectTransform.anchoredPosition = Screen.safeArea.position;
            rectTransform.sizeDelta = Screen.safeArea.size * (1/ rectTransform.lossyScale.x);
        }
    }
}
