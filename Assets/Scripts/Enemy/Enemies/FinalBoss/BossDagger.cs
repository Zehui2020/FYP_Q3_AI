using DesignPatterns.ObjectPool;
using System.Collections;
using UnityEngine;

public class BossDagger : PooledObject
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float speed;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform lineStatPos;

    private BaseStats target;
    private BaseStats thrower;
    private float damage;

    private Rigidbody2D daggerRB;
    private Coroutine moveRoutine;
    private Coroutine rotateRoutine;

    public event System.Action OnReleased;

    private bool isMoving = false;
    private bool isParried = false;

    public override void Init()
    {
        base.Init();
        daggerRB = GetComponent<Rigidbody2D>();
        lineRenderer.enabled = false;
    }

    public void SetupDagger(float damage, BaseStats thrower)
    {
        this.damage = damage;
        this.thrower = thrower;
        target = PlayerController.Instance;
    }

    public void ShootDagger()
    {
        StartCoroutine(ShootRoutine());
        StartCoroutine(ReleaseRoutine());
    }

    private IEnumerator RotateRoutine()
    {
        float rotateDuration = 1f;
        Vector3 dirToTarget = Vector2.zero;

        while (true)
        {
            rotateDuration -= Time.deltaTime;
            dirToTarget = Vector3.Normalize(transform.position - target.transform.position);
            transform.up = Vector3.Lerp(transform.up, dirToTarget, Time.deltaTime * 5f);
            yield return null;
        }
    }

    private IEnumerator ShootRoutine()
    {
        rotateRoutine = StartCoroutine(RotateRoutine());

        float timer = 1f;
        while (timer > 0)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, lineStatPos.position);
            lineRenderer.SetPosition(1, target.transform.position);
            timer -= Time.deltaTime;
            yield return null;
        }

        StopCoroutine(rotateRoutine);
        rotateRoutine = null;
        moveRoutine = StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        isMoving = true;
        transform.parent = null;
        Vector3 dirToTarget = Vector3.Normalize(transform.position - target.transform.position);
        transform.up = dirToTarget;

        AudioManager.Instance.PlayOneShot(Sound.SoundName.BossP1Knife);
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
                lineRenderer.enabled = false;

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

        if (Utility.CheckLayer(collision.gameObject, groundLayer))
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
        lineRenderer.enabled = false;

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        if (rotateRoutine != null)
            StopCoroutine(rotateRoutine);

        rotateRoutine = null;
        moveRoutine = null;
    }
}