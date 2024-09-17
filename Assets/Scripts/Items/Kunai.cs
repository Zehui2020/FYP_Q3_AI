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
        gameObject.SetActive(true);

        thrower = baseStats;

        if (direction == Vector2.right)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);

        kunaiRB.AddForce(direction * force, ForceMode2D.Impulse);

        StartCoroutine(ReleaseRoutine());
    }

    private IEnumerator ReleaseRoutine()
    {
        yield return new WaitForSeconds(5f);

        Release();
        gameObject.SetActive(false);
    }
}