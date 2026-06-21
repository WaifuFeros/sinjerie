using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(Image))]
public class UIAspectKeeper : MonoBehaviour
{
    private Image _image;
    private RectTransform _rectTransform;

    void OnEnable()
    {
        _image = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();
        UpdateShaderAspect();
    }
    void OnRectTransformDimensionsChange()
    {
        UpdateShaderAspect();
    }

    void UpdateShaderAspect()
    {
        if (_image == null || _rectTransform == null) return;

        // On récupère le matériau appliqué à l'image
        if (_image.material != null && _image.material.HasProperty("_AspectRatio"))
        {
            float width = _rectTransform.rect.width;
            float height = _rectTransform.rect.height;

            if (height > 0)
            {
                float aspect = width / height;
                _image.material.SetFloat("_AspectRatio", aspect);
            }
        }
    }

#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying)
        {
            UpdateShaderAspect();
        }
    }
#endif
}