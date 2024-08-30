using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;

public class ItemPickup : PooledObject
{
    public enum RotateAxis
    {
        Up,
        Forward,
        Right
    }
    public RotateAxis rotateAxis;

    [SerializeField] private Item item;
    [SerializeField] private float rotationSpeed;

    public override void InitPrefab()
    {
        objectName = item.title;
    }

    public void PickupItem()
    {
        //PlayerController.Instance.AddItem(item);

        Release();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        switch (rotateAxis)
        {
            case RotateAxis.Up:
                transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
                break;
            case RotateAxis.Forward:
                transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
                break;
            case RotateAxis.Right:
                transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
                break;
        }
    }
}