using UnityEngine;

public class TitleName : MonoBehaviour
{
    private Animator animator;
    private int intervalToGlitch;
    private int counter;

    private RectTransform rectTransform;
    [SerializeField] private float maxY;
    [SerializeField] private float minY;
    [SerializeField] private float bobSpeed;

    private float originalY;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        animator = GetComponent<Animator>();

        intervalToGlitch = Random.Range(3, 8);
        originalY = rectTransform.anchoredPosition.y;
    }

    public void DecideTitle()
    {
        if (counter >= intervalToGlitch)
        {
            animator.SetTrigger("glitch");
            counter = 0;
            intervalToGlitch = Random.Range(3, 10);
        }
        else
        {
            animator.SetTrigger("normal");
            counter++;
        }
    }

    private void Update()
    {
        float newY = originalY + Mathf.Sin(Time.time * bobSpeed) * (maxY - minY) / 2;
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, newY);
    }
}