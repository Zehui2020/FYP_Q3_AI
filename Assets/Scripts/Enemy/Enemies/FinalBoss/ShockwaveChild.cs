using UnityEngine;
using UnityEngine.Events;

public class ShockwaveChild : MonoBehaviour
{
    public UnityEvent<Collider2D> OnTriggerEnter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerEnter?.Invoke(collision);
    }
}