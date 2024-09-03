using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCollisionController : MonoBehaviour
{
    [SerializeField] private CombatCollisionTrigger[] colliders;
    [SerializeField] LayerMask targetLayer;

    private BaseStats stats;
    private float damage;
    private int critRate;
    private float critMultiplier;

    private void Start()
    {
        stats = GetComponent<BaseStats>();
        foreach (var collider in colliders)
        {
            collider.TriggerEvent += TriggerEvent;
            collider.SetCollider(false);
        }
    }

    public void EnableCollider(float newDamage, int col)
    {
        damage = newDamage;
        critRate = stats.critRate;
        critMultiplier = stats.critDamage;
        colliders[col].SetCollider(true);
    }

    public void EnableCollider(float newDamage, int critRate, float critMultiplier, int col)
    {
        damage = newDamage;
        this.critRate = critRate;
        this.critMultiplier = critMultiplier;
        colliders[col].SetCollider(true);
    }

    public void DisableCollider(int col)
    {
        damage = 0;
        colliders[col].SetCollider(false);
    }

    public void DisableAllCollider()
    {
        damage = 0;
        foreach (CombatCollisionTrigger col in colliders)
            col.SetCollider(false);
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    private void TriggerEvent(Collider2D col)
    {
        if (!Utility.Instance.CheckLayer(col.gameObject, targetLayer))
            return;

        Vector3 closestPoint = col.ClosestPoint(transform.position);

        GameObject hitGameObject = col.gameObject;
        EnemyStats enemyStats = Utility.Instance.GetTopmostParent(hitGameObject.transform).GetComponentInChildren<EnemyStats>();
        PlayerStats playerStats = hitGameObject.GetComponent<PlayerStats>();

        if (enemyStats != null)
        {
            enemyStats.TakeDamage(damage, critRate, critMultiplier, closestPoint);
            DisableAllCollider();
        }
        else if (playerStats != null)
        {
            playerStats.TakeDamage(damage, critRate, critMultiplier, closestPoint);
        }
    }
}