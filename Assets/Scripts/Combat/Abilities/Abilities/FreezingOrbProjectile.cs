using UnityEngine;

public class FreezingOrbProjectile : AbilityProjectile
{
    [SerializeField] private GameObject areaObj;
    [SerializeField] private AreaOfEffect areaOfEffect;
    [SerializeField] private int range;

    protected override void OnHit(BaseStats target)
    {
        areaObj.SetActive(true);
        areaObj.transform.SetParent(null);
        areaOfEffect.InitParticles(5, 0.1f, 1);
        areaOfEffect.HandleStatusOverTime();
        for (int i = 0; i < areaOfEffect.particleVFXManager.Count; i++)
        {
            areaOfEffect.particleVFXManager[i].OnFrozen();
        }

        base.OnHit(target);
    }
}
