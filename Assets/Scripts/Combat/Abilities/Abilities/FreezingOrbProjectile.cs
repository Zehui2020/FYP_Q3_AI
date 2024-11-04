using UnityEngine;

public class FreezingOrbProjectile : AbilityProjectile
{
    [SerializeField] private float range;

    protected override void OnHit(BaseStats target)
    {
        InitParticles(10, range, 3);
        AudioManager.Instance.PlayOneShot(Sound.SoundName.FreezingOrb);
        base.OnHit(target);
    }
}
