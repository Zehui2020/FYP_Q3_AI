using UnityEngine;

public class FreezingOrbProjectile : AbilityProjectile
{
    [SerializeField] private float range;

    protected override void OnHit(BaseStats target)
    {
        InitParticles(10, range, 3);

        base.OnHit(target);
    }
}
