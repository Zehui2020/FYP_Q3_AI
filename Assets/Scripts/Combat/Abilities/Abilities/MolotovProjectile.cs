using UnityEngine;

public class MolotovProjectile : AbilityProjectile
{
    [SerializeField] private float range;

    protected override void OnHit(BaseStats target)
    {
        InitParticles(10, range, 1.5f);

        base.OnHit(target);
    }
}
