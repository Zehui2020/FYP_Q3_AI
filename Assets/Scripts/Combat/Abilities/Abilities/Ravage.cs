using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Ravage")]
public class Ravage : BaseAbility
{
    [SerializeField] private GameObject ravageVFX;

    public override void InitAbility()
    {
    }

    public override void OnAbilityUse(BaseStats singleTarget, List<BaseStats> targetList)
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            BaseStats target = targetList[i];
            Instantiate(ravageVFX, target.transform, false);
        }

        AudioManager.Instance.PlayOneShot(Sound.SoundName.Ravage);
    }

    public override void OnAbilityEnd(BaseStats singleTarget, List<BaseStats> targetList)
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            BaseStats target = targetList[i];
            target.ApplyStatusEffect(
                new StatusEffect.StatusType(
                    StatusEffect.StatusType.Type.Debuff,
                    StatusEffect.StatusType.Status.Static
                    ),
                (int)abilityStrength
                );

            target.particleVFXManager.OnStunned();
        }
    }
}
