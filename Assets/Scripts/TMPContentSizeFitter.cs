using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TMPContentSizeFitter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    private ContentSizeFitter contentSizeFitter;
    private float preferredHeight;

    private void Awake()
    {
        contentSizeFitter = GetComponent<ContentSizeFitter>();
        contentSizeFitter.enabled = false;
        preferredHeight = textComponent.rectTransform.rect.height;
    }

    private void Update()
    {
        bool isOverflowing = textComponent.preferredHeight > preferredHeight;
        contentSizeFitter.enabled = isOverflowing;
    }
}