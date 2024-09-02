using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [SerializeField] private WeaponData wData;

    private AnimationManager animationManager;

    private Coroutine attackRoutine;

    public void InitializeMovementController()
    {
        animationManager = GetComponent<AnimationManager>();
    }
    public void HandleAttack()
    {
        if (attackRoutine == null)
            attackRoutine = StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        animationManager.ChangeAnimation(animationManager.Attacking, 0, 0, true);

        yield return new WaitForSeconds(wData.attackSpeed);

        attackRoutine = null;
    }
}
