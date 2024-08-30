using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;

public class ItemPickup : PooledObject
{
    [SerializeField] private Item item;
    [SerializeField] private float bobIntensity = 0.5f;
    [SerializeField] private float bobSpeed = 2f;
    private Vector3 startPos;

    public override void InitPrefab()
    {
        objectName = item.title;
        startPos = transform.position;
    }

    public void PickupItem()
    {
        ItemManager.Instance.AddItem(item);

        Release();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobIntensity;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
            PickupItem();
    }
}