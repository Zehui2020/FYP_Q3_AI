using DesignPatterns.ObjectPool;
using System.Collections;
using UnityEngine;

public class Kunai : PooledObject
{
    private BaseStats thrower;
    [SerializeField] private Rigidbody2D kunaiRB;
    [SerializeField] private ItemStats itemStats;
    [SerializeField] private float force;
    [SerializeField] private LayerMask targetLayer;

    public void SetupKunai(BaseStats baseStats, Vector2 direction)
    {
        thrower = baseStats;

        kunaiRB.AddForce(direction * force, ForceMode2D.Impulse);
        if (direction == Vector2.left)
            transform.localScale = new Vector3(-1, 1, 1);

        StartCoroutine(ReleaseRoutine());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(Utility.Instance.CheckLayer(collision.gameObject, targetLayer));

        if (!Utility.Instance.CheckLayer(collision.gameObject, targetLayer))
            return;

        if (!collision.TryGetComponent<EnemyStats>(out EnemyStats target))
            return;

        BaseStats.Damage damage = target.CalculateProccDamageDealt(target, new BaseStats.Damage(thrower.attack * itemStats.kunaiDamageMultiplier), out bool crit, out DamagePopup.DamageType damageType);
        target.TakeDamage(thrower, damage, crit, target.transform.position, damageType);

        Release();
        gameObject.SetActive(false);
    }

    private IEnumerator ReleaseRoutine()
    {
        yield return new WaitForSeconds(5f);

        Release();
        gameObject.SetActive(false);
    }
}