using UnityEngine;

public class MolotovProjectile : AbilityProjectile
{
    [SerializeField] private float range;

    protected override void OnHit(BaseStats target)
    {
        InitParticles(10, range, 1.5f);

        // get all target objects in area
        Collider2D[] targetColliders = Physics2D.OverlapCircleAll(target.transform.position, 2, targetLayer);

        foreach (Collider2D col in targetColliders)
        {
            BaseStats targetInArea = col.GetComponent<BaseStats>();
            if (target != null)
                targetInArea.ApplyStatusEffect(
                    new StatusEffect.StatusType(
                        StatusEffect.StatusType.Type.Debuff,
                        StatusEffect.StatusType.Status.Burn
                        ),
                    8
                    );
        }

        AudioManager.Instance.PlayOneShot(Sound.SoundName.MolotovCocktail);
        base.OnHit(target);
    }
}
