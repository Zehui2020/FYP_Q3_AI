using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Divine Blessing")]
public class DivineBlessing : BaseAbility
{
    public override void OnUseAbility(BaseStats self, BaseStats target)
    {
        // remove status effects
        self.Cleanse(StatusEffect.StatusType.Type.Debuff);
    }
}