using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Heat Wave")]
public class HeatWave : BaseAbility
{
    [SerializeField] GameObject wavePrefab;
    public override void InitAbility()
    {
    }

    public override void OnAbilityUse(BaseStats singleTarget, List<BaseStats> targetList)
    {
        GameObject obj = Instantiate(wavePrefab);
        obj.transform.position = PlayerController.Instance.transform.position;
        obj.transform.localScale = new Vector3(PlayerController.Instance.transform.localScale.x, 1, 1);
        Vector3 force = new Vector3(PlayerController.Instance.transform.localScale.x * abilityRange + PlayerController.Instance.GetComponent<Rigidbody2D>().velocity.x, 0, 0);
        obj.GetComponent<HeatWaveProjectile>().LaunchProjectile(force);
    }

    public override void OnAbilityEnd(BaseStats singleTarget, List<BaseStats> targetList)
    {
    }
}
