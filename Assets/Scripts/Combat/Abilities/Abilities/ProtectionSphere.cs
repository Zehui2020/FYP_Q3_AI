using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Protection Sphere")]
public class ProtectionSphere : BaseAbility
{
    [SerializeField] LayerMask targetLayer;
    private int count;

    public override void InitAbility()
    {
    }

    public override void OnAbilityUse(BaseStats self, BaseStats target)
    {
        count = (int)(10 / abilityDuration);
        PlayerController.Instance.abilityController.TriggerOverlayAnim(0.4f, "ProtectionSphere");
    }

    private void OnAbilityLoop(BaseStats self, BaseStats target)
    {
        PushTargets();
        PlayerController.Instance.abilityController.HandleAbilityDuration(this, self, target);
    }

    public override void OnAbilityEnd(BaseStats self, BaseStats target)
    {
        if (count <= 0)
        {
            PlayerController.Instance.abilityController.TriggerOverlayAnim(0.4f, "ProtectionSphereOff");
            PlayerController.Instance.abilityController.abilityOverlayAnimator.gameObject.SetActive(false);
            return;
        }

        OnAbilityLoop(self, target);
    }

    private void PushTargets()
    {
        // get all target objects in area
        Collider2D[] targetColliders = Physics2D.OverlapCircleAll(PlayerController.Instance.transform.position, abilityRange, targetLayer);
        foreach (Collider2D col in targetColliders)
        {
            BaseStats target = col.GetComponent<BaseStats>();
            if (target != null)
            {
                // push targets away
                float dir = target.transform.position.x < PlayerController.Instance.transform.position.x ? -1 : 1;
                target.GetComponent<Rigidbody2D>().velocity = new Vector3(dir * abilityStrength, abilityStrength, 0);
                target.particleVFXManager.OnStunned();
            }
        }
        count--;
    }
}