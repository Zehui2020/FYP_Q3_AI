using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Heat Wave")]
public class HeatWave : BaseAbility
{
    [SerializeField] GameObject wavePrefab;

    public override void OnAbilityUse(BaseStats self, BaseStats target)
    {
        GameObject obj = Instantiate(wavePrefab);
        obj.transform.position = PlayerController.Instance.transform.position;
        Vector3 force = new Vector3(PlayerController.Instance.transform.localScale.x * abilityRange + PlayerController.Instance.GetComponent<Rigidbody2D>().velocity.x, 0, 0);
        obj.GetComponent<HeatWaveProjectile>().LaunchProjectile(force);
    }

    public override void OnAbilityEnd(BaseStats self, BaseStats target)
    {
    }
}
