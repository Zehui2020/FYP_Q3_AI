using DesignPatterns.ObjectPool;
using System.Collections;
using UnityEngine;

public class BossDagger : PooledObject
{
    [SerializeField] private Vector2 randLifetime;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float speed;

    private BaseStats target;
    private BaseStats thrower;
    private float damage;

    private Rigidbody2D daggerRB;
    private Coroutine moveRoutine;

    public event System.Action OnReleased;

    private bool isMoving = false;
    private bool isParried = false;

    public override void Init()
    {
        base.Init();
        daggerRB = GetComponent<Rigidbody2D>();
    }

    public void SetupDagger(float damage, BaseStats thrower)
    {
        this.damage = damage;
        this.thrower = thrower;
        target = PlayerController.Instance;
        float lifetime = Random.Range(randLifetime.x, randLifetime.y);
        StartCoroutine(ShootRoutine(lifetime));
        StartCoroutine(ReleaseRoutine());
    }

    private IEnumerator ShootRoutine(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);

        float rotateDuration = 1f;
        Vector3 dirToTarget = Vector2.zero;

        while (Vector2.Distance(transform.up, dirToTarget) > 0.1f)
        {
            rotateDuration -= Time.deltaTime;
            dirToTarget = Vector3.Normalize(transform.position - target.transform.position);
            transform.up = Vector3.Lerp(transform.up, dirToTarget, Time.deltaTime * 5f);
            yield return null;
        }

        moveRoutine = StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        isMoving = true;
        transform.parent = null;
        Vector3 dirToTarget = Vector3.Normalize(transform.position - target.transform.position);
        transform.up = dirToTarget;

        while (true)
        {
            daggerRB.velocity = -dirToTarget * speed;
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isMoving)
            return;

        if (collision.TryGetComponent<PlayerController>(out PlayerController player))
        {
            if (player.immuneType == BaseStats.ImmuneType.Parry)
            {
                player.OnParryEnemy(thrower);
                target = thrower;
                isParried = true;

                if (moveRoutine != null)
                    StopCoroutine(moveRoutine);
                moveRoutine = StartCoroutine(MoveRoutine());

                return;
            }

            player.TakeDamage(thrower, new BaseStats.Damage(damage), false, player.transform.position, DamagePopup.DamageType.Health);
            ReleaseDagger();
        }

        if (collision.TryGetComponent<FinalBossPhase1>(out FinalBossPhase1 boss) && isParried)
        {
            boss.TakeDamage(PlayerController.Instance, new BaseStats.Damage(damage * 5f), false, boss.transform.position, DamagePopup.DamageType.Health);
            ReleaseDagger();
        }

        if (Utility.Instance.CheckLayer(collision.gameObject, groundLayer))
            ReleaseDagger();
    }

    private IEnumerator ReleaseRoutine()
    {
        yield return new WaitForSeconds(15f);

        ReleaseDagger();
    }

    private void OnDisable()
    {
        OnReleased = null;
    }

    private void ReleaseDagger()
    {
        OnReleased?.Invoke();
        isMoving = false;
        isParried = false;
        Release();
        gameObject.SetActive(false);
    }
}