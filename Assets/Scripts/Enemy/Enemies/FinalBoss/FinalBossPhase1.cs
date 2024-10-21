using DesignPatterns.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossPhase1 : Enemy
{
    public enum State
    {
        Idle,
        Walk,
        SlamAttack,
        Rush,
        PunchAttack,
        DaggerAttack
    }

    private readonly int IdleAnim = Animator.StringToHash("BossP1Idle");
    private readonly int WalkAnim = Animator.StringToHash("BossP1Walk");

    private readonly int JumpAnim = Animator.StringToHash("BossP1Jump");
    private readonly int SlamAnim = Animator.StringToHash("BossP1Slam");

    private readonly int RushAnim = Animator.StringToHash("BossP1Rush");
    private readonly int PunchAnim = Animator.StringToHash("BossP1Punch");

    private readonly int DaggerAnim = Animator.StringToHash("BossP1Dagger");

    [Header("Final Boss Stats")]
    public State currentState;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float rushSpeed;

    [SerializeField] private float jumpSpeed;
    [SerializeField] private Transform jumpWaitPosition;
    [SerializeField] private float jumpWaitDuration;

    [SerializeField] private float movesTillSlam;

    [Header("Damage Stats")]
    [SerializeField] private float punchDamageModifier;
    [SerializeField] private float slamDamageModifier;
    [SerializeField] private float daggerDamageModifier;
    [SerializeField] private List<Transform> daggerSpawnPos = new();
    [SerializeField] private List<BossDagger> bossDaggers = new();

    private int moveCounter;
    private Coroutine SlamRoutine;
    private Coroutine Deciding;

    // Debugging
    private State previousState;

    public void ChangeState(State newState)
    {
        currentState = newState;
        damageMultipler.RemoveAllModifiers();

        switch (currentState)
        {
            case State.Idle:
                animator.Play(IdleAnim);
                break;
            case State.Walk:
                animator.Play(WalkAnim);
                break;
            case State.SlamAttack:
                SlamRoutine = StartCoroutine(SlamAttack());
                break;
            case State.Rush:
                animator.Play(RushAnim);
                break;
            case State.PunchAttack:
                damageMultipler.AddModifier(punchDamageModifier);
                moveCounter++;
                enemyRB.velocity = Vector2.zero;
                animator.Play(PunchAnim);
                break;
            case State.DaggerAttack:
                animator.Play(DaggerAnim);
                for (int i = 0; i < daggerSpawnPos.Count; i++)
                {
                    BossDagger dagger = ObjectPool.Instance.GetPooledObject("BossDagger", true) as BossDagger;
                    dagger.SetupDagger(CalculateDamageDealt(player, Damage.DamageSource.Normal, out bool crit, out DamagePopup.DamageType damageType), this);

                    dagger.transform.SetParent(daggerSpawnPos[i]);
                    dagger.transform.localPosition = Vector3.zero;
                    dagger.transform.localRotation = Quaternion.identity;
                    dagger.transform.localScale = Vector3.one;

                    dagger.OnReleased += () => { bossDaggers.Remove(dagger); };
                    bossDaggers.Add(dagger);
                }
                break;
        }
    }

    public override bool AttackTarget(BaseStats target, Damage.DamageSource damageSource, Vector3 closestPoint)
    {
        bool damagedTarget = base.AttackTarget(target, damageSource, closestPoint);
        if (damagedTarget)
            player.movementController.Knockback(30f);
        
        return damagedTarget;
    }

    public override void UpdateEnemy()
    {
        base.UpdateEnemy();
        switch (currentState)
        {
            case State.Walk:
                enemyRB.velocity = GetDirectionToPlayer() * walkSpeed;
                break;
            case State.Rush:
                enemyRB.velocity = GetDirectionToPlayer() * rushSpeed;
                if (Vector2.Distance(transform.position, player.transform.position) <= 2f)
                    ChangeState(State.PunchAttack);
                break;
        }

        if (Input.GetKeyDown(KeyCode.L))
            ChangeState(State.Walk);

        if (currentState != State.PunchAttack)
            UpdateDirectionToPlayer();
    }

    public void Decide()
    {
        if (Deciding != null)
            StopCoroutine(Deciding);

        Deciding = StartCoroutine(DecideRoutine());
    }

    private IEnumerator DecideRoutine()
    {
        animator.Play(IdleAnim);
        enemyRB.velocity = Vector2.zero;

        yield return new WaitForSeconds(2f);

        if (moveCounter >= movesTillSlam)
        {
            ChangeState(State.SlamAttack);
            moveCounter = 0;
            Deciding = null;
            yield break;
        }

        if (bossDaggers.Count == 0)
            ChangeState(State.DaggerAttack);
        else
            ChangeState(State.Rush);

        Deciding = null;
    }

    private IEnumerator SlamAttack()
    {
        animator.Play(JumpAnim);
        damageMultipler.AddModifier(slamDamageModifier);

        yield return new WaitForSeconds(jumpWaitDuration);

        Vector3 targetPos = player.transform.position;

        while (true)
        {
            Vector3 direction = (targetPos - transform.position).normalized;
            Vector3 newPos = transform.position + direction * jumpSpeed * Time.deltaTime;

            enemyRB.MovePosition(newPos);

            yield return null;
        }
    }

    public void OnJump()
    {
        QuestPointer.Instance.Show(transform);
        StartCoroutine(JumpRoutine());
    }
    private IEnumerator JumpRoutine()
    {
        player.playerEffectsController.ShakeCamera(4f, 3f, 0.3f);
        float timer = 1f;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            enemyRB.MovePosition(transform.position + Vector3.up * Time.deltaTime * jumpSpeed);
            yield return null;
        }
    }

    private void OnValidate()
    {
        if (currentState != previousState)
        {
            previousState = currentState;
            ChangeState(previousState);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Utility.Instance.CheckLayer(collision.gameObject, groundLayer) && currentState == State.SlamAttack)
        {
            if (SlamRoutine != null)
                StopCoroutine(SlamRoutine);

            QuestPointer.Instance.Hide();
            animator.Play(SlamAnim);
            SlamRoutine = null;
            player.playerEffectsController.ShakeCamera(8f, 5f, 0.6f);
        }
    }
}