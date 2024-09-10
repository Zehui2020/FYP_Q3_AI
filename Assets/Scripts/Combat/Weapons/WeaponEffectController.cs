using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEffectController : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> weaponPSs;
    private ParticleSystem.VelocityOverLifetimeModule velocityModule;

    [SerializeField] private Transform player;

    public void PlayPS(int currentCombo)
    {
        velocityModule = weaponPSs[currentCombo].velocityOverLifetime;

        float playerDirection = Mathf.Sign(player.localScale.x);
        velocityModule.x = new ParticleSystem.MinMaxCurve(Mathf.Abs(velocityModule.x.constantMin) * playerDirection, Mathf.Abs(velocityModule.x.constantMax) * playerDirection);

        weaponPSs[currentCombo].Play();
    }
}