using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatWaveProjectile : AbilityProjectile
{
    [SerializeField] private float lifeTime;

    private void Start()
    {
        StartCoroutine(DeathRoutine());
    }

    protected override void OnHit(BaseStats target)
    {
        target.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Burn), 1);
    }

    public IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
