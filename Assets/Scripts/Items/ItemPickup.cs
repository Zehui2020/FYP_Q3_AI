using UnityEngine;
using DesignPatterns.ObjectPool;

public class ItemPickup : PooledObject
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Item item;
    private ObjectBobbing bobbing;

    private void OnEnable()
    {
        if (bobbing == null)
            return;

        bobbing.InitBobbing();
    }

    public void InitPickup(Item newItem)
    {
        bobbing = GetComponent<ObjectBobbing>();
        bobbing.InitBobbing();

        item = newItem;
        spriteRenderer.sprite = item.spriteIcon;
        spriteRenderer.material = newItem.itemOutlineMaterial;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent<ItemManager>(out ItemManager itemManager))
        {
            itemManager.AddItem(item);
            Release();
            gameObject.SetActive(false);
        }
    }
}