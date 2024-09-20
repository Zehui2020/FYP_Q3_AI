using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCollisionTrigger : MonoBehaviour
{
    [SerializeField] private BaseStats.Damage.DamageSource damageSource;
    [SerializeField] private Collider2D col;

    public event System.Action<Collider2D, BaseStats.Damage.DamageSource> TriggerEvent;

    public void SetCollider(bool enable)
    {
        col.enabled = enable;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        TriggerEvent?.Invoke(col, damageSource);
    }

    private void OnDisable()
    {
        TriggerEvent = null;
    }
}