using DesignPatterns.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private GameObject shadowPillar;

    [SerializeField] private int armsToSummon;

    private readonly int IdleAnim = Animator.StringToHash("BossP2Idle");
    private readonly int LaserAnim = Animator.StringToHash("BossP2Laser");
    private readonly int SmashAnim = Animator.StringToHash("BossP2Smash");
    private readonly int SummonArmsAnim = Animator.StringToHash("BossP2SummonArms");
    private readonly int SummonPillarAnim = Animator.StringToHash("BossP2SummonPillar");

    public override void InitializeEnemy()
    {
        base.InitializeEnemy();
        currentSegment = 4;
        segmentMinHealth = Mathf.CeilToInt(maxHealth / maxSegments * currentSegment);
    }

    public void ChangeState(State newState)
    {
        currentState = newState;
        damageMultipler.RemoveAllModifiers();

        switch (currentState)
        {
            case State.Idle:
                animator.Play(IdleAnim);
                break;
            case State.LaserAttack:
                animator.Play(LaserAnim);
                DoLaserAttack();
                break;
            case State.SmashAttack:
                animator.Play(SmashAnim);
                break;
            case State.SummonArms:
                animator.Play(SummonArmsAnim);
                break;
            case State.SummonPillars:
                animator.Play(SummonPillarAnim);
                break;
        }
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

    private void DoLaserAttack()
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

        yield return new WaitForSeconds(2f);

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