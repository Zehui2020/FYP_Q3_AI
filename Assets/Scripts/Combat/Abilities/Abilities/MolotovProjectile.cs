using UnityEngine;

public class MolotovProjectile : AbilityProjectile
{
    [SerializeField] private GameObject areaObj;
    [SerializeField] private AreaOfEffect areaOfEffect;
    [SerializeField] private int range;

    protected override void OnHit(BaseStats target)
    {
        areaObj.SetActive(true);
        areaObj.transform.SetParent(null);
        areaOfEffect.InitParticles(10, 0.05f, 4);
        areaOfEffect.HandleStatusOverTime();
        for (int i = 0; i < areaOfEffect.particleVFXManager.Count; i++)
        {
            areaOfEffect.particleVFXManager[i].OnBurning(0);
        }

        base.OnHit(target);
    }
}
