using UnityEngine;

public class IgnoreParentScale : MonoBehaviour
{
    [SerializeField] private Transform parent;

    void LateUpdate()
    {
        transform.localScale = new Vector3(Mathf.Sign(parent.localScale.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}