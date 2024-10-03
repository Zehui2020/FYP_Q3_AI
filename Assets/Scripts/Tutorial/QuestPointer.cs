using UnityEngine;
using UnityEngine.UI;

public class QuestPointer : MonoBehaviour
{
    private Camera mainCam;
    [SerializeField] private Sprite arrowSprite;
    [SerializeField] private Sprite crossSprite;

    [SerializeField] private Transform targetPosition;
    [SerializeField] private RectTransform pointerRectTransform;
    [SerializeField] private Image pointerImage;

    [SerializeField] private float borderSize = 100f;

    private void Start()
    {
        mainCam = Camera.main;
        Hide();
    }

    private void Update()
    {
        if (targetPosition == null)
            return;

        Vector3 targetPositionScreenPoint = mainCam.WorldToScreenPoint(targetPosition.position);
        bool isOffScreen = targetPositionScreenPoint.x <= borderSize ||
                           targetPositionScreenPoint.x >= Screen.width - borderSize ||
                           targetPositionScreenPoint.y <= borderSize ||
                           targetPositionScreenPoint.y >= Screen.height - borderSize;

        if (isOffScreen)
        {
            RotatePointerTowardsTargetPosition();
            pointerImage.sprite = arrowSprite;

            Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
            cappedTargetScreenPosition.x = Mathf.Clamp(cappedTargetScreenPosition.x, borderSize, Screen.width - borderSize);
            cappedTargetScreenPosition.y = Mathf.Clamp(cappedTargetScreenPosition.y, borderSize, Screen.height - borderSize);

            pointerRectTransform.position = cappedTargetScreenPosition;
        }
        else
        {
            pointerImage.sprite = crossSprite;
            pointerRectTransform.position = targetPositionScreenPoint;
            pointerRectTransform.localEulerAngles = Vector3.zero;
        }
    }

    private void RotatePointerTowardsTargetPosition()
    {
        Vector3 toPosition = targetPosition.position;
        Vector3 fromPosition = mainCam.transform.position;
        fromPosition.z = 0f;
        Vector3 dir = (toPosition - fromPosition).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        pointerRectTransform.localEulerAngles = new Vector3(0, 0, angle);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show(Transform targetPosition)
    {
        gameObject.SetActive(true);
        this.targetPosition = targetPosition;
    }
}