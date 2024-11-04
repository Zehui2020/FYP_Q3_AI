using UnityEngine;

public class MolotovProjectile : AbilityProjectile
{
    [SerializeField] private float range;

    protected override void OnHit(BaseStats target)
    {
        InitParticles(10, range, 1.5f);
        for (int i = 0; i < particleVFXManager.Count; i++)
        {
            particleVFXManager[i].OnBurning(0);
        }

        AudioManager.Instance.PlayOneShot(Sound.SoundName.MolotovCocktail);
        base.OnHit(target);
    }
}
