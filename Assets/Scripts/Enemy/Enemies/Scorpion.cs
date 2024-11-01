using DesignPatterns.ObjectPool;
using System.Collections;
using UnityEngine;

public class Scorpion : Enemy
{
    public enum State
    {
        Idle,
        Patrol,
        Melee,
        Throw,
        Hurt,
        Die
    }
    public State currentState;

    private readonly int IdleAnim = Animator.StringToHash("ScorpionIdle");
    private readonly int RunAnim = Animator.StringToHash("ScorpionWalk");
    private readonly int MeleeAnim = Animator.StringToHash("ScorpionMelee");
    private readonly int ThrowAnim = Animator.StringToHash("ScorpionThrow");
    private readonly int HurtAnim = Animator.StringToHash("ScorpionHurt");
    private readonly int DieAnim = Animator.StringToHash("ScorpionDie");

    [Header("Scorpion Stats")]
    [SerializeField] private AudioProxy audioProxy;
    [SerializeField] private float meleeRange;
    [SerializeField] private float throwCooldown;
    [SerializeField] private Transform bombSpawnPos;
    [SerializeField] private float throwDistance;
    private bool canThrow = true;

    public override void InitializeEnemy()
    {
        base.InitializeEnemy();

        ChangeState(State.Patrol);

        onReachWaypoint += () => { ChangeState(State.Idle); };
        onFinishIdle += () => { ChangeState(State.Patrol); };
        OnBreached += (multiplier) => { animator.Play(IdleAnim); };
        OnParry += (stat) => { animator.Play(IdleAnim); };
        onHitEvent += (target, damage, crit, pos) => { if (CheckHurt()) ChangeState(State.Hurt); };
    }

    public override bool AttackTarget(BaseStats target, Damage.DamageSource damageSource, Vector3 closestPoint)
    {
        bool tookDamage = base.AttackTarget(target, damageSource, closestPoint);
        if (tookDamage)
        {
            int randNum = Random.Range(0, 2);
            if (randNum == 0)
                target.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Poison), 1);
        }
        return tookDamage;
    }

    public override void OnDie()
    {
        base.OnDie();
        ChangeState(State.Die);
    }

    private void ChangeState(State newState)
    {
        if (currentState == State.Die)
            return;

        if (health <= 0 && newState != State.Die)
            return;

        currentState = newState;

        if (currentState == State.Patrol)
            audioProxy.PlayAudioOneShot(Sound.SoundName.ScorpionWalk);
        else
            audioProxy.StopAudioInstantly(Sound.SoundName.ScorpionWalk);

        switch (newState)
        {
            case State.Idle:
                animator.CrossFade(IdleAnim, 0f);
                Idle();
                break;
            case State.Patrol:
                animator.CrossFade(RunAnim, 0f);
                break;
            case State.Melee:
                UpdateDirectionToPlayer();
                aiNavigation.StopNavigation();
                animator.Play(MeleeAnim, -1, 0f);
                break;
            case State.Throw:
                UpdateDirectionToPlayer();
                aiNavigation.StopNavigation();
                animator.Play(ThrowAnim, -1, 0f);
                break;
            case State.Hurt:
                aiNavigation.StopNavigation();
                animator.Play(HurtAnim, -1, 0f);
                break;
            case State.Die:
                animator.speed = 1;
                animator.CrossFade(DieAnim, 0f);
                aiNavigation.StopNavigation();
                uiController.SetCanvasActive(false);
                break;
        }
    }

    public override void UpdateEnemy()
    {
        base.UpdateEnemy();

        if (currentState == State.Throw &&
            Vector2.Distance(player.transform.position, transform.position) >= throwDistance)
        {
            ChangeState(State.Patrol);
            isInCombat = false;
        }
        else if (Vector2.Distance(player.transform.position, transform.position) <= meleeRange && 
            currentState != State.Melee && 
            currentState != State.Throw &&
            currentState != State.Hurt)
            ChangeState(State.Melee);
        else if (isInCombat && 
            canThrow &&
            currentState != State.Throw &&
            currentState != State.Melee &&
            currentState != State.Hurt &&
            Vector2.Distance(player.transform.position, transform.position) < throwDistance)
            ChangeState(State.Throw);
        else if (canThrow &&
            CheckChasePlayer() &&
            currentState != State.Throw &&
            currentState != State.Melee &&
            currentState != State.Hurt)
            ChangeState(State.Throw);

        switch (currentState)
        {
            case State.Idle:
                CheckChasePlayer();
                break;
            case State.Patrol:
                if (CheckChasePlayer())
                    return;

                PatrolUpdate();
                break;
        }

        if (currentState != State.Melee &&
            currentState != State.Throw)
            UpdateMovementDirection();
    }

    public void ThrowBomb()
    {
        StartCoroutine(ThrowRoutine());
    }

    private IEnumerator ThrowRoutine()
    {
        canThrow = false;

        ScorpionBomb bomb = ObjectPool.Instance.GetPooledObject("ScorpionBomb", false) as ScorpionBomb;
        bomb.transform.position = bombSpawnPos.position;
        bomb.InitScorpionBomb(this, bombSpawnPos.position, PlayerController.Instance.transform.position);

        yield return new WaitForSeconds(throwCooldown);

        canThrow = true;
    }

    public override void Knockback(float force)
    {
        enemyRB.velocity = Vector2.zero;
        base.Knockback(force);
    }

    public override bool TakeDamage(BaseStats attacker, Damage damage, bool isCrit, Vector3 closestPoint, DamagePopup.DamageType damageType)
    {
        bool tookDamage = base.TakeDamage(attacker, damage, isCrit, closestPoint, damageType);

        if (health <= 0)
            ChangeState(State.Die);

        return tookDamage;
    }
}