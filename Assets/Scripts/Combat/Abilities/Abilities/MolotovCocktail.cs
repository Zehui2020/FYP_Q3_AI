using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Molotov Cocktail")]
public class MolotovCocktail : BaseAbility
{
    [SerializeField] GameObject nadePrefab;

    public override void OnAbilityUse(BaseStats self, BaseStats target)
    {
        GameObject obj = Instantiate(nadePrefab);
        obj.transform.position = PlayerController.Instance.transform.position;
        Vector3 force = new Vector3(PlayerController.Instance.transform.localScale.x * abilityRange + PlayerController.Instance.GetComponent<Rigidbody2D>().velocity.x, abilityStrength, 0);
        obj.GetComponent<MolotovProjectile>().LaunchProjectile(force);
    }

    public override void OnAbilityEnd(BaseStats self, BaseStats target)
    {
    }
}
