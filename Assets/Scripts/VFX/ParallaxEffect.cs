using System.Collections;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] private Transform followPos;
    [SerializeField] private Transform player;
    [SerializeField] private float parallaxEffect;

    private SpriteRenderer spriteRenderer;
    [SerializeField] private string filename;
    private ImageSaver imageSaver;

    private float mapOriginY;
    private float mapSizeY;

    [SerializeField] private float YDiff;

    public void InitParallaxEffect(float mapSizeY)
    {
        this.mapSizeY = mapSizeY;

        mapOriginY = mapSizeY / 2f;
    }

    private void Awake()
    {
        imageSaver = GetComponent<ImageSaver>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = imageSaver.GetSpriteFromLocalDisk(filename);
    }

    private void LateUpdate()
    {
        // Parallax Y
        if (mapSizeY == 0)
            return;

        float diff = transform.position.y - mapOriginY;
        float offsetY = Mathf.Abs(transform.position.y - mapSizeY) / mapSizeY * YDiff;
        offsetY = diff < 0 ? offsetY : -offsetY;
        offsetY = Mathf.Clamp(offsetY, -YDiff, YDiff);
        transform.position = new Vector3(transform.position.x, followPos.transform.position.y + offsetY, 0);
    }
}