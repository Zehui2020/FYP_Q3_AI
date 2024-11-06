using DesignPatterns.ObjectPool;
using UnityEngine;

public class PlayerAfterimage : PooledObject
{
    [SerializeField] private SpriteRenderer imageSR;

    public void SetupImage(Sprite sprite, float scaleXSign)
    {
        imageSR.sprite = sprite;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * scaleXSign, transform.localScale.y, transform.localScale.z);
    }

    public void ReleaseAfterimage()
    {
        Release();
        gameObject.SetActive(false);
    }
}