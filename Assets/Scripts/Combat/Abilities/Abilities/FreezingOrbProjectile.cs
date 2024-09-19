using UnityEngine;

public class FreezingOrbProjectile : AbilityProjectile
{
    [SerializeField] private GameObject areaObj;
    [SerializeField] private AreaOfEffect areaOfEffect;
    [SerializeField] private int range;

    protected override void OnHit(BaseStats target)
    {
        areaObj.SetActive(true);
        areaObj.transform.localScale = new Vector3(range / transform.localScale.x, areaObj.transform.localScale.y / transform.localScale.y, areaObj.transform.localScale.z);
        areaObj.transform.SetParent(transform.parent);
        areaOfEffect.HandleStatusOverTime();

        base.OnHit(target);
    }
}
