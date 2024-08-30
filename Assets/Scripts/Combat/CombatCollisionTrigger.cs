using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCollisionTrigger : MonoBehaviour
{
    [SerializeField] private Collider2D col;

    public event System.Action<Collider2D> TriggerEvent;

    public void SetCollider(bool enable)
    {
        col.enabled = enable;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        TriggerEvent?.Invoke(col);
    }

    private void OnDisable()
    {
        TriggerEvent = null;
    }
}