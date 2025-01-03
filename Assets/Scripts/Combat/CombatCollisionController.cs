using UnityEngine;

public class CombatCollisionController : MonoBehaviour
{
    [SerializeField] private CombatCollisionTrigger[] colliders;
    [SerializeField] LayerMask targetLayer;

    private BaseStats attacker;

    public void InitCollisionController(BaseStats stats)
    {
        attacker = stats;
        foreach (var collider in colliders)
        {
            collider.TriggerEvent += TriggerEvent;
            collider.SetCollider(false);
        }
    }

    public void EnableCollider(int col)
    {
        colliders[col].SetCollider(true);
    }

    public void DisableCollider(int col)
    {
        colliders[col].SetCollider(false);
    }

    public void DisableAllCollider()
    {
        foreach (CombatCollisionTrigger col in colliders)
            col.SetCollider(false);
    }

    private void TriggerEvent(Collider2D col, BaseStats.Damage.DamageSource damageSource)
    {
        if (!Utility.CheckLayer(col.gameObject, targetLayer))
            return;

        Vector3 closestPoint = col.ClosestPoint(transform.position);

        BaseStats target = col.GetComponent<BaseStats>();

        if (target != null)
        {
            attacker.AttackTarget(target, damageSource, closestPoint);
            DisableAllCollider();
        }
    }
}