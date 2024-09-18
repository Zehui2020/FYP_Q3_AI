using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MolotovProjectile : AbilityProjectile
{
    [SerializeField] private GameObject areaObj;
    [SerializeField] private AreaOfEffect areaOfEffect;
    [SerializeField] private int range;

    protected override void OnHit(BaseStats target)
    {
        areaObj.SetActive(true);
        areaObj.transform.localScale = new Vector3(range, areaObj.transform.localScale.y, areaObj.transform.localScale.z);
        areaObj.transform.SetParent(transform.parent);
        areaOfEffect.HandleStatusOverTime();

        base.OnHit(target);
    }
}
