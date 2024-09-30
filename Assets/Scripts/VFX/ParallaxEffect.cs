using System.Collections;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private float length;

    private Vector2 startPos;
    [SerializeField] private Transform followPos;
    [SerializeField] private Transform player;
    [SerializeField] private float parallaxEffect;

    [SerializeField] private SpriteRenderer[] spriteRenderers;
    [SerializeField] private string filename;
    private ImageSaver imageSaver;

    private float mapOriginY;
    private float mapSizeY;

    [SerializeField] private float YDiff;

    public void InitParallaxEffect(float mapSizeY, Vector2 startPos)
    {
        this.mapSizeY = mapSizeY;
        this.startPos = startPos;

        mapOriginY = mapSizeY / 2f;
    }

    private void Awake()
    {
        imageSaver = GetComponent<ImageSaver>();
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            spriteRenderer.sprite = imageSaver.GetSpriteFromLocalDisk(filename);

        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void LateUpdate()
    {
        // Parallax X
        float temp = followPos.transform.position.x * (1 - parallaxEffect);

        if (temp > startPos.x + length)
            startPos.x += length;
        else if (temp < startPos.x - length)
            startPos.x -= length;

        float dist = followPos.transform.position.x * parallaxEffect;
        transform.position = new Vector3(startPos.x + dist, transform.position.y, 0);

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