using DesignPatterns.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class FinalBossPhase2 : Enemy
{
    public enum State
    {
        Idle,
        LaserAttack,
        PunchAttack,
        SmashAttack,
        SummonArms,
        SummonPillars
    }

    public enum LaserType
    {
        Swiping,
        Stationary,
        Moving,
        TotalTypes
    }

    [Header("Boss Stats")]
    public State currentState;
    private State previousState;

    private int segmentMinHealth;
    private int currentSegment;
    [SerializeField] private int maxSegments;

    [SerializeField] private Transform laserStartPoint;
    [SerializeField] private Transform shockwaveSpawnPoint;
    [SerializeField] private List<Transform> handSpawnPoint;
    [SerializeField] private List<Transform> pillarSpawnPoint;
    [SerializeField] private List<Transform> handPunchSpawnPoint;
    [SerializeField] private GameObject shadowPillar;
    [SerializeField] private Animator healthBarAnimator;

    [SerializeField] private int movesToLaser;
    [SerializeField] private int handChance;
    [SerializeField] private int armsToSummon;

    [Header("Others")]
    [SerializeField] private CutsceneGroup cutscene;
    [SerializeField] private VisualEffect laserVFX;
    private int moveCounter;

    private Coroutine Deciding;

    private readonly int IdleAnim = Animator.StringToHash("BossP2Idle");
    private readonly int LaserAnim = Animator.StringToHash("BossP2Laser");
    private readonly int LaserEndAnim = Animator.StringToHash("BossP2LaserEnd");
    private readonly int SmashAnim = Animator.StringToHash("BossP2Smash");
    private readonly int PunchAnim = Animator.StringToHash("BossP2Punch");
    private readonly int PunchEndAnim = Animator.StringToHash("BossP2PunchEnd");
    private readonly int SummonArmsAnim = Animator.StringToHash("BossP2SummonArms");
    private readonly int SummonPillarAnim = Animator.StringToHash("BossP2SummonPillar");

    public override void InitializeEnemy()
    {
        base.InitializeEnemy();
        currentSegment = 4;
        segmentMinHealth = Mathf.CeilToInt(maxHealth / maxSegments * currentSegment);

        cutscene.CutsceneEnd.AddListener(() => { ChangeState(State.LaserAttack); });
    }

    public void InitBarUI()
    {
        healthBarAnimator.enabled = false;
        uiController.InitUIController(this);
    }

    public void ChangeState(State newState)
    {
        //if (health <= 0 && newState != State.Die)
        //    return;

        currentState = newState;
        damageMultipler.RemoveAllModifiers();

        switch (currentState)
        {
            case State.Idle:
                animator.Play(IdleAnim);
                break;
            case State.LaserAttack:
                animator.Play(LaserAnim);
                break;
            case State.SmashAttack:
                animator.Play(SmashAnim);
                break;
            case State.PunchAttack:
                animator.Play(PunchAnim);
                break;
            case State.SummonArms:
                animator.Play(SummonArmsAnim);
                break;
            case State.SummonPillars:
                animator.Play(SummonPillarAnim);
                break;
        }
    }

    public void Decide()
    {
        if (Deciding != null)
            StopCoroutine(Deciding);

        Deciding = StartCoroutine(DecideRoutine());
    }
    private IEnumerator DecideRoutine()
    {
        ChangeState(State.Idle);
        enemyRB.velocity = Vector2.zero;

        yield return new WaitForSeconds(2f);

        if (moveCounter >= movesToLaser)
        {
            ChangeState(State.LaserAttack);
            Deciding = null;
            yield break;
        }

        int doHands = Random.Range(0, 100);
        if (doHands < handChance)
            ChangeState(State.SummonArms);
        else
        {
            int randNum = Random.Range(0, 100);
            if (randNum < 50)
                ChangeState(State.SmashAttack);
            else
                ChangeState(State.PunchAttack);
        }

        Deciding = null;
    }

    public override bool TakeDamage(BaseStats attacker, Damage damage, bool isCrit, Vector3 closestPoint, DamagePopup.DamageType damageType)
    {
        bool tookDamage = base.TakeDamage(attacker, damage, isCrit, closestPoint, damageType);
        CheckSummonPillars();
        return tookDamage;
    }
    public override void TakeTrueDamage(Damage damage)
    {
        base.TakeTrueDamage(damage);
        CheckSummonPillars();
    }

    private void CheckSummonPillars()
    {
        if (health <= segmentMinHealth)
        {
            ChangeState(State.SummonPillars);
            currentSegment--;
            segmentMinHealth = Mathf.CeilToInt(maxHealth / maxSegments * currentSegment);
        }
    }

    public override void UpdateEnemy()
    {
        base.UpdateEnemy();
    }

    public void DoLaserAttack()
    {
        StartCoroutine(LaserSwipeRoutine());
    }
    private IEnumerator LaserSwipeRoutine()
    {
        float timer = 5f;
        float elapsed = 0f;

        Vector2 dirToPlayer = (player.transform.position - laserStartPoint.position).normalized;
        float startAngle = Utility.GetAngleFromDirection(dirToPlayer);
        float targetAngle = transform.position.x < player.transform.position.x ? 200 : -200;

        BossLaser bossLaser = ObjectPool.Instance.GetPooledObject("BossLaser", true) as BossLaser;
        bossLaser.SetupLaser(
            laserStartPoint.position,
            Physics2D.Raycast(laserStartPoint.position, dirToPlayer, 100, groundLayer).point
        );

        while (elapsed < timer)
        {
            float t = elapsed / timer;
            float currentAngle = Mathf.Lerp(startAngle, targetAngle, t);

            RaycastHit2D[] hits = Physics2D.RaycastAll(
                laserStartPoint.position,
                Utility.GetDirectionFromAngle(currentAngle)
            );

            if (hits.Length > 0)
            {
                System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
                RaycastHit2D lastHit = hits[hits.Length - 1];

                bossLaser.UpdateLaser(laserStartPoint.position, lastHit.point);

                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.TryGetComponent<PlayerController>(out PlayerController player))
                    {
                        player.TakeDamage(
                            this,
                            new Damage(Damage.DamageSource.Unparriable, attack),
                            false,
                            player.transform.position,
                            DamagePopup.DamageType.Health
                        );
                    }
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        animator.Play(LaserEndAnim, 0, 0);
        bossLaser.ReleaseLaser();
    }

    public void SummonShockwaves()
    {
        player.playerEffectsController.ShakeCamera(10f, 4f, 0.6f);

        Shockwave shockwave = ObjectPool.Instance.GetPooledObject("Shockwave", true) as Shockwave;
        shockwave.InitShockwave(this,
            CalculateDamageDealt(player, Damage.DamageSource.Normal, out bool crit, out DamagePopup.DamageType damageType),
            shockwaveSpawnPoint.position);
    }

    public void SummonArms()
    {
        for (int i = 0; i < armsToSummon; i++)
        {
            ShadowHand shadowHand = ObjectPool.Instance.GetPooledObject("ShadowHand", true) as ShadowHand;
            shadowHand.InitShadowHand();
            shadowHand.transform.position = handSpawnPoint[i].position;
        }
    }

    private void SummonPillars()
    {
        float randX = Random.Range(pillarSpawnPoint[0].position.x, pillarSpawnPoint[1].position.x);
        Instantiate(shadowPillar, new Vector3(randX, pillarSpawnPoint[1].position.y, 0), Quaternion.identity);
    }

    private void OnValidate()
    {
        if (currentState != previousState)
        {
            previousState = currentState;
            ChangeState(previousState);
        }
    }

    public void SummonPunchArm()
    {
        int randNum = Random.Range(0, 2);

        BossPunchHand bossPunchHand = ObjectPool.Instance.GetPooledObject("BossPunchHand", true) as BossPunchHand;
        bossPunchHand.InitHand(this, handPunchSpawnPoint[randNum].position.x < transform.position.x ? Vector3.right : Vector3.left);
        bossPunchHand.transform.position = handPunchSpawnPoint[randNum].position;
    }
    public void PunchEnd()
    {
        animator.Play(PunchEndAnim, 0, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Crystal"))
            return;

        player.playerEffectsController.ShakeCamera(10f, 4f, 0.6f);
        TriggerStatusState(StatusEffect.StatusType.Status.Dazed, shieldRegenDelay);
        TakeDamage(this, new Damage(maxHealth * 0.08f), false, transform.position, DamagePopup.DamageType.Health);
        Destroy(collision.gameObject);
    }
}