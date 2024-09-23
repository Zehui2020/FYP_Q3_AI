using System.Collections.Generic;
using UnityEngine;

public class WeaponEffectController : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> weaponPSs;
    private ParticleSystem.VelocityOverLifetimeModule velocityModule;

    [System.Serializable]
    public struct AxeShakeValues
    {
        public float intensity;
        public float frequency;
        public float duration;
    }

    [SerializeField] private List<AxeShakeValues> axeShake;
    [SerializeField] private PlayerEffectsController playerEffectsController;

    [SerializeField] private Transform player;

    public void PlayPS(int currentCombo)
    {
        velocityModule = weaponPSs[currentCombo].velocityOverLifetime;

        float playerDirection = Mathf.Sign(player.localScale.x);
        velocityModule.x = new ParticleSystem.MinMaxCurve(Mathf.Abs(velocityModule.x.constantMin) * playerDirection, Mathf.Abs(velocityModule.x.constantMax) * playerDirection);

        weaponPSs[currentCombo].Play();
    }

    public void AxeShake(int index)
    {
        playerEffectsController.ShakeCamera(axeShake[index].intensity, axeShake[index].frequency, axeShake[index].duration);
    }
}