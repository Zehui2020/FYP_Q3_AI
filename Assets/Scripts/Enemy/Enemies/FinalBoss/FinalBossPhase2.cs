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
        SummonPillars,
        Die
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
    private int prevSegmentMinHealth;
    private int currentSegment;
    [SerializeField] private int maxSegments;
    [SerializeField] private LevelManager levelManager;
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
    [SerializeField] private LineRenderer lineRenderer;
    private int moveCounter;

    private Coroutine Deciding;
    private Coroutine LaserRoutine;

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
        prevSegmentMinHealth = Mathf.CeilToInt(maxHealth / maxSegments * currentSegment);

        laserVFX.enabled = false;

        OnDieEvent += (die) => { ChangeState(State.Die); StopAllCoroutines(); laserVFX.enabled = false; };
    }

    public void InitBarUI()
    {
        healthBarAnimator.enabled = false;
        uiController.InitUIController(this);
    }

    public void ChangeState(State newState)
    {
        if (health <= 0 && newState != State.Die && currentState != State.LaserAttack)
            return;

        currentState = newState;
        damageMultipler.RemoveAllModifiers();

        switch (currentState)
        {
            case State.Idle:
                animator.Play(IdleAnim);
                break;
            case State.LaserAttack:
                animator.Play(LaserAnim);
                AudioManager.Instance.PlayOneShot(Sound.SoundName.BossP2Roar);
                break;
            case State.SmashAttack:
                animator.Play(SmashAnim);
                AudioManager.Instance.PlayOneShot(Sound.SoundName.BossP2Roar);
                break;
            case State.PunchAttack:
                animator.Play(PunchAnim);
                AudioManager.Instance.PlayOneShot(Sound.SoundName.BossP2Roar);
                break;
            case State.SummonArms:
                animator.Play(SummonArmsAnim);
                break;
            case State.SummonPillars:
                if (LaserRoutine != null)
                {
                    StopCoroutine(LaserRoutine);
                    LaserRoutine = null;
                    laserVFX.enabled = false;
                }

                ApplyImmune(1000000, ImmuneType.BossImmune);
                animator.Play(SummonPillarAnim);
                break;
            case State.Die:
                StopAllCoroutines();
                cutscene.EnterCutscene();
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
            moveCounter = 0;
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

        moveCounter++;

        Deciding = null;
    }

    public override bool TakeDamage(BaseStats attacker, Damage damage, bool isCrit, Vector3 closestPoint, DamagePopup.DamageType damageType)
    {
        if (health - damage.damage <= prevSegmentMinHealth)
            damage = new Damage(health - prevSegmentMinHealth);

        bool tookDamage = base.TakeDamage(attacker, damage, isCrit, closestPoint, damageType);
        CheckSummonPillars();
        return tookDamage;
    }
    public override void TakeTrueDamage(Damage damage)
    {
        if (health - damage.damage <= prevSegmentMinHealth)
            damage = new Damage(health - prevSegmentMinHealth);

        base.TakeTrueDamage(damage);
        CheckSummonPillars();
    }
    public override void TakeTrueShieldDamage(Damage damage)
    {
        base.TakeTrueShieldDamage(damage);
        CheckSummonPillars();
    }

    private void CheckSummonPillars()
    {
        if (health <= segmentMinHealth && health > 0)
        {
            ChangeState(State.SummonPillars);
            currentSegment--;
            prevSegmentMinHealth = segmentMinHealth;
            segmentMinHealth = Mathf.CeilToInt(maxHealth / maxSegments * currentSegment);
            SummonPillars();
        }
    }

    public override void UpdateEnemy()
    {
        base.UpdateEnemy();
    }

    public void DoLaser()
    {
        ChangeState(State.LaserAttack);
    }

    public void DoLaserAttack()
    {
        LaserRoutine = StartCoroutine(LaserSwipeRoutine());
    }
    private IEnumerator LaserSwipeRoutine()
    {
        float trackTimer = 2.08f;
        float timer = 5f;
        float elapsed = 0f;

        Vector2 dirToPlayer = (player.transform.position - laserStartPoint.position).normalized;
        float startAngle = Utility.GetAngleFromDirection(dirToPlayer);
        float targetAngle = transform.position.x < player.transform.position.x ? 200 : -200;

        while (trackTimer > 0)
        {
            float t = elapsed / 1.08f;
            trackTimer -= Time.deltaTime;

            dirToPlayer = (player.transform.position - laserStartPoint.position).normalized;
            startAngle = Utility.GetAngleFromDirection(dirToPlayer);
            targetAngle = transform.position.x < player.transform.position.x ? 200 : -200;
            float currentAngle = Mathf.Lerp(startAngle, targetAngle, t);

            RaycastHit2D[] hits = Physics2D.RaycastAll(
                laserStartPoint.position,
                Utility.GetDirectionFromAngle(currentAngle)
            );

            foreach (RaycastHit2D hit in hits)
            {
                if (Utility.CheckLayer(hit.collider.gameObject, groundLayer))
                {
                    lineRenderer.SetPosition(0, laserStartPoint.position);
                    lineRenderer.SetPosition(1, hit.point);

                    break;
                }
            }

            yield return null;
        }

        yield return new WaitForSeconds(1f);

        elapsed = 0;
        laserVFX.enabled = true;
        lineRenderer.enabled = false;

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

                foreach (RaycastHit2D hit in hits)
                {
                    if (Utility.CheckLayer(hit.collider.gameObject, groundLayer))
                    {
                        laserVFX.SetVector3("StartPosition_position", laserStartPoint.position);
                        laserVFX.SetVector3("EndPosition_position", hit.point);
                        break;
                    }
                }

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

            if (!AudioManager.Instance.CheckIfSoundPlaying(Sound.SoundName.BossP2Laser))
                AudioManager.Instance.PlayOneShot(Sound.SoundName.BossP2Laser);
            elapsed += Time.deltaTime;
            yield return null;
        }

        laserVFX.enabled = false;
        animator.Play(LaserEndAnim, 0, 0);
        LaserRoutine = null;
    }

    public void SummonShockwaves()
    {
        AudioManager.Instance.PlayOneShot(Sound.SoundName.BossP2Roar);
        player.playerEffectsController.ShakeCamera(10f, 4f, 0.6f);

        Shockwave shockwave = ObjectPool.Instance.GetPooledObject("Shockwave", true) as Shockwave;
        shockwave.InitShockwave(this,
            CalculateDamageDealt(player, Damage.DamageSource.Normal, out bool crit, out DamagePopup.DamageType damageType),
            shockwaveSpawnPoint.position);
    }

    public void SummonArms()
    {
        AudioManager.Instance.PlayOneShot(Sound.SoundName.BossP2Summon);
        for (int i = 0; i < armsToSummon; i++)
        {
            ShadowHand shadowHand = ObjectPool.Instance.GetPooledObject("ShadowHand", true) as ShadowHand;
            shadowHand.InitShadowHand();
            shadowHand.transform.position = handSpawnPoint[i].position;
        }
    }

    private void SummonPillars()
    {
        AudioManager.Instance.PlayOneShot(Sound.SoundName.BossP2Summon);
        float randX = Random.Range(pillarSpawnPoint[0].position.x, pillarSpawnPoint[1].position.x);
        Instantiate(shadowPillar, new Vector3(randX, pillarSpawnPoint[1].position.y, 0), Quaternion.identity);
        levelManager.ChangeRandomTheme();
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

        AudioManager.Instance.PlayOneShot(Sound.SoundName.BossP2Punch);
        BossPunchHand bossPunchHand = ObjectPool.Instance.GetPooledObject("BossPunchHand", true) as BossPunchHand;
        bossPunchHand.InitHand(this, handPunchSpawnPoint[randNum].position.x < player.transform.position.x ? Vector3.left : Vector3.right);
        bossPunchHand.transform.position = handPunchSpawnPoint[randNum].position;
    }
    public void PunchEnd()
    {
        animator.Play(PunchEndAnim, 0, 0);
        AudioManager.Instance.PlayOneShot(Sound.SoundName.BossP2Roar);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Crystal"))
            return;

        ApplyImmune(100, ImmuneType.None);
        prevSegmentMinHealth = segmentMinHealth;

        player.playerEffectsController.ShakeCamera(10f, 4f, 0.6f);
        TriggerStatusState(StatusEffect.StatusType.Status.Dazed, shieldRegenDelay);
        TakeDamage(this, new Damage(maxHealth * 0.08f), false, transform.position, DamagePopup.DamageType.Health);
        Destroy(collision.gameObject);

        ApplyImmune(100, ImmuneType.None);
    }
}