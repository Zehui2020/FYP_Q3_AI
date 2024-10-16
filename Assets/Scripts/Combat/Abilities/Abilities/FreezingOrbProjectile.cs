using UnityEngine;

public class FreezingOrbProjectile : AbilityProjectile
{
    [SerializeField] private float range;

    protected override void OnHit(BaseStats target)
    {
        InitParticles(10, range, 3);
        for (int i = 0; i < particleVFXManager.Count; i++)
        {
            particleVFXManager[i].OnFrozen();
        }

        base.OnHit(target);
    }
}
