using UnityEngine;

public class IgnoreParent : MonoBehaviour
{
    private Vector3 initialWorldPosition;

    void Awake()
    {
        initialWorldPosition = transform.position;
    }

    void LateUpdate()
    {
        transform.position = initialWorldPosition;
    }
}