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
        SummonDagger,
        DaggerAttack,
        Die
    }

    private readonly int IdleAnim = Animator.StringToHash("BossP1Idle");
    private readonly int WalkAnim = Animator.StringToHash("BossP1Walk");

    private readonly int JumpAnim = Animator.StringToHash("BossP1Jump");
    private readonly int SlamAnim = Animator.StringToHash("BossP1Slam");

    private readonly int RushAnim = Animator.StringToHash("BossP1Rush");
    private readonly int PunchAnim = Animator.StringToHash("BossP1Punch");

    private readonly int DaggerSummonAnim = Animator.StringToHash("BossP1StartSummonDagger");
    private readonly int DaggerAttackAnim = Animator.StringToHash("BossP1DaggerAttack");

    [Header("Final Boss Stats")]
    public State currentState;
    private State prevState;
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

    [Header("Others")]
    [SerializeField] private CutsceneGroup cutscene;
    [SerializeField] private CutsceneGroup dieCutscene;
    [SerializeField] private GameObject nextPhase;

    private int moveCounter;
    private bool canCharge = false;
    private Coroutine SlamRoutine;
    private Coroutine Deciding;

    // Debugging
    private State previousState;

    public override void InitializeEnemy()
    {
        base.InitializeEnemy();
        OnBreached += (amount) => {
            animator.Play(IdleAnim);
        };
        OnParry += (baseStats) => {
            animator.Play(IdleAnim);
        };

        cutscene.CutsceneEnd.AddListener(() => { ChangeState(State.Walk); });
        dieCutscene.CutsceneEnd.AddListener(() => { nextPhase.SetActive(true); transform.parent.gameObject.SetActive(false); });
    }

    public void ChangeState(State newState)
    {
        if (newState == currentState || currentState == State.Die)
            return;

        if (newState != State.Idle)
        {
            isInCombat = true;
            uiController.SetCanvasActive(true);
        }

        prevState = currentState;
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
            case State.SummonDagger:
                StartCoroutine(SummonDaggerRoutine());
                break;
            case State.DaggerAttack:
                moveCounter++;
                animator.Play(DaggerAttackAnim);
                bossDaggers[0].ShootDagger();
                break;
            case State.Die:
                dieCutscene.EnterCutscene();
                break;
        }
    }

    private IEnumerator SummonDaggerRoutine()
    {
        animator.Play(DaggerSummonAnim);
        enemyRB.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.7f);

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

            yield return new WaitForSeconds(0.5f);
        }
        ChangeState(State.DaggerAttack);
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
                if (!canCharge)
                    return;
                enemyRB.velocity = GetDirectionToPlayer() * rushSpeed;
                if (Vector2.Distance(transform.position, player.transform.position) <= 2f)
                {
                    canCharge = false;
                    ChangeState(State.PunchAttack);
                }
                break;
        }

        if (currentState != State.PunchAttack)
            UpdateDirectionToPlayer();
    }

    public void Decide()
    {
        if (Deciding != null)
            StopCoroutine(Deciding);

        Deciding = StartCoroutine(DecideRoutine());
    }

    public void SetCanCharge()
    {
        canCharge = true;
    }

    private IEnumerator DecideRoutine()
    {
        ChangeState(State.Idle);
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
            ChangeState(State.SummonDagger);
        else
        {
            if (prevState == State.DaggerAttack)
                ChangeState(State.Rush);
            else
                ChangeState(State.DaggerAttack);
        }

        Deciding = null;
    }

    private IEnumerator SlamAttack()
    {
        animator.Play(JumpAnim);
        damageMultipler.AddModifier(slamDamageModifier);

        yield return new WaitForSeconds(jumpWaitDuration);

        Vector3 targetPos = Physics2D.Raycast(transform.position, Vector2.down, 1000f, groundLayer).point;
        enemyRB.isKinematic = false;

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
        transform.position = jumpWaitPosition.position;
        enemyRB.isKinematic = true;
    }

    public override void OnDie()
    {
        base.OnDie();
        ChangeState(State.Die);
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
        if (Utility.CheckLayer(collision.gameObject, groundLayer) && currentState == State.SlamAttack)
        {
            player.playerEffectsController.ShakeCamera(10f, 4f, 0.6f);

            if (SlamRoutine != null)
                StopCoroutine(SlamRoutine);

            Shockwave shockwave = ObjectPool.Instance.GetPooledObject("Shockwave", true) as Shockwave;
            shockwave.InitShockwave(this, 
                CalculateDamageDealt(player, Damage.DamageSource.Normal, out bool crit, out DamagePopup.DamageType damageType),
                collision.contacts[0].point);

            QuestPointer.Instance.Hide();
            animator.Play(SlamAnim);
            SlamRoutine = null;
        }
    }
}