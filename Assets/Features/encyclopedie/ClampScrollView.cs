using UnityEngine;

public class ClampScrollView : MonoBehaviour
{
    public RectTransform content;

    void LateUpdate()
    {
        Vector2 pos = content.anchoredPosition;
        pos.x = 0;
        content.anchoredPosition = pos;
    }
}
