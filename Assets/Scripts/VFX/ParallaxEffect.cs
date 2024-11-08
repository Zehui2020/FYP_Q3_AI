using System.Collections;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private Vector2 startPos;
    private Animator animator;

    [SerializeField] private bool centerPivot = false;
    [SerializeField] private bool updateY;
    [SerializeField] private Transform followPos;
    [SerializeField] private Transform player;
    [SerializeField] private float parallaxEffect;

    private SpriteRenderer spriteRenderer;
    [SerializeField] private string levelName;
    [SerializeField] private string filename;
    private ImageSaver imageSaver;

    private float mapOriginY;
    private float mapSizeY;

    [SerializeField] private float YDiff;

    private void Start()
    {
        imageSaver = GetComponent<ImageSaver>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        Vector2 pivot = centerPivot ? new Vector2(0.5f, 0.5f) : new Vector2(0.5f, 0);

        if (levelName == string.Empty)
            spriteRenderer.sprite = imageSaver.GetSpriteFromLocalDisk(filename + "_" + GameData.Instance.currentLevel, pivot);
        else
        {
            spriteRenderer.sprite = imageSaver.GetSpriteFromLocalDisk(filename + "_" + levelName, pivot);
            animator.SetBool("fadeOut", true);
        }
    }

    public void InitParallaxEffect(float mapSizeY)
    {
        this.mapSizeY = mapSizeY;

        mapOriginY = mapSizeY / 2f;
        if (updateY)
            transform.position = player.transform.position;

        startPos = transform.position;
    }

    public void Fade(bool fadeOut)
    {
        animator.SetBool("fadeOut", fadeOut);
    }

    public void UpdateParallax()
    {
        // Parallax X
        float dist = followPos.transform.position.x * parallaxEffect;
        transform.position = new Vector3(startPos.x - dist, transform.position.y, transform.position.z);

        // Parallax Y
        if (mapSizeY == 0 || !updateY)
            return;

        float diff = transform.position.y - mapOriginY;
        float offsetY = Mathf.Abs(transform.position.y - mapSizeY) / mapSizeY * YDiff;
        offsetY = diff < 0 ? offsetY * parallaxEffect : -offsetY * parallaxEffect;
        offsetY = Mathf.Clamp(offsetY, -YDiff, YDiff);
        transform.position = new Vector3(transform.position.x, followPos.transform.position.y + offsetY, transform.position.z);
    }

    public void ChangeSprite(string level)
    {
        Vector2 pivot = centerPivot ? new Vector2(0.5f, 0.5f) : new Vector2(0.5f, 0);
        spriteRenderer.sprite = imageSaver.GetSpriteFromLocalDisk(filename + "_" + level, pivot);
    }
}