using UnityEngine;

namespace WMG.Utilities
{
    public class UIPanel : MonoBehaviour
    {
        [SerializeField] private PanelFitMode _fit;
        [SerializeField] private bool _update;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = transform as RectTransform;
        }

        private void Start()
        {
            FitPanel();
        }

        private void Update()
        {
            if (_update)
            {
                FitPanel();
            }
        }

        private void FitPanel()
        {
            switch (_fit)
            {
                default:
                case PanelFitMode.None:
                    break;
                case PanelFitMode.FitToScreen:
                    SetPanelOffsets(_rectTransform, 0, 0, 0, 0);
                    break;
                case PanelFitMode.safeArea:
                    ApplySafeArea(_rectTransform);
                    break;
            }
        }

        private void SetPanelOffsets(RectTransform rectTransform, float leftOffset = 0, float rightOffset = 0, float topOffset = 0, float bottomOffset = 0)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;

            rectTransform.offsetMin = new Vector2(leftOffset, bottomOffset);
            rectTransform.offsetMax = new Vector2(-rightOffset, -topOffset);
        }

        public static void ApplySafeArea(RectTransform rectTransform, float leftOffset = 0, float rightOffset = 0, float topOffset = 0, float bottomOffset = 0)
        {
            Rect safeArea = Screen.safeArea;

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;

            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;

            rectTransform.offsetMin = new Vector2(leftOffset, bottomOffset);
            rectTransform.offsetMax = new Vector2(-rightOffset, -topOffset);
        }

        internal enum PanelFitMode
        {
            None,
            FitToScreen,
            safeArea
        }
    }
}
