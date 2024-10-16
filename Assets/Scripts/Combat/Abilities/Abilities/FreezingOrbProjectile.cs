using UnityEngine;

public class FreezingOrbProjectile : AbilityProjectile
{
    [SerializeField] private int range;

    protected override void OnHit(BaseStats target)
    {
        InitParticles(10, 0.2f, 2);
        for (int i = 0; i < particleVFXManager.Count; i++)
        {
            particleVFXManager[i].OnFrozen();
        }

        base.OnHit(target);
    }
}
