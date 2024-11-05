using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Shred")]
public class Shred : BaseAbility
{
    [SerializeField] private GameObject shredVFX;
    [SerializeField] private int damageCount;
    private int count;

    public override void InitAbility()
    {
    }

    public override void OnAbilityUse(BaseStats singleTarget, List<BaseStats> targetList)
    {
        count = damageCount;
        for (int i = 0; i < targetList.Count; i++)
        {
            BaseStats target = targetList[i];
            // on the ground
            if (target.GetComponent<Rigidbody2D>().velocity.y == 0)
            {
                // deal damage
                target.TakeTrueDamage(new BaseStats.Damage(GetDamage()));
                // push targets away
                if (!target.canAbilityKnockback)
                    continue;
                target.GetComponent<Rigidbody2D>().velocity = new Vector3(0, abilityRange * 2, 0);
                GameObject obj = Instantiate(shredVFX, target.transform, false);
                obj.transform.localEulerAngles = new Vector3(0, 0, 90);
                AudioManager.Instance.PlayOneShot(Sound.SoundName.Shred);
            }
        }
    }

    public override void OnAbilityEnd(BaseStats singleTarget, List<BaseStats> targetList)
    {
        if (count <= 0)
            return;

        for (int i = 0; i < targetList.Count; i++)
        {
            BaseStats target = targetList[i];
            // deal damage
            target.particleVFXManager.StopStatic();
            target.TakeTrueDamage(new BaseStats.Damage(GetDamage() / 10));
            // push targets away
            if (!target.canAbilityKnockback)
                continue;
            float dir = Random.Range(0, 2) == 0 ? -1 : 1;
            target.GetComponent<Rigidbody2D>().velocity = new Vector3(dir * abilityRange, abilityRange, 0);
            GameObject obj = Instantiate(shredVFX, target.transform, false);
            obj.transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360));
            AudioManager.Instance.PlayOneShot(Sound.SoundName.Shred);
        }
        count--;

        OnAbilityLoop(singleTarget, targetList);
    }
}