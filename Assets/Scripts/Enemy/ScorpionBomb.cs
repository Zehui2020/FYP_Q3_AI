using DesignPatterns.ObjectPool;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScorpionBomb : PooledObject
{
    private Rigidbody2D bombRB;
    private BoxCollider2D bombCol;
    private Animator animator;

    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float radius;
    [SerializeField] private float speed;
    [SerializeField] private float maxArcHeight;
    [SerializeField] private float damageMultToSelf;

    private Scorpion thrower;
    private PlayerController player;
    private Vector3 targetPos;

    private bool isParried = false;

    public override void Init()
    {
        base.Init();
        bombRB = GetComponent<Rigidbody2D>();
        bombCol = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    public void InitScorpionBomb(Scorpion thrower, Vector2 startPos, Vector2 targetPos)
    {
        gameObject.SetActive(true);
        bombCol.isTrigger = true;

        this.thrower = thrower;
        player = PlayerController.Instance;

        LaunchBomb(startPos, targetPos, 1);
    }

    private void LaunchBomb(Vector2 startPos, Vector2 targetPos, float arcModifier)
    {
        this.targetPos = targetPos;

        RaycastHit2D hit = Physics2D.Raycast(targetPos, Vector2.down, 100f, groundLayer);
        if (hit)
            this.targetPos = new Vector2(targetPos.x, hit.point.y);

        float maxArc = maxArcHeight * arcModifier;
        // Check for ground and adjust arc height if necessary
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, -Vector2.down * maxArc, maxArc, groundLayer);
        if (hits.Length > 0)
            maxArc = hits[hits.Length - 1].distance;

        float distance = targetPos.x - startPos.x;
        float peakHeight = Mathf.Max(startPos.y, targetPos.y) + maxArc;

        float gravity = Mathf.Abs(Physics2D.gravity.y * bombRB.gravityScale);
        float vy = Mathf.Sqrt(2 * gravity * (peakHeight - startPos.y));

        float timeToPeak = vy / gravity;
        float totalTime = 2 * timeToPeak;

        float vx = distance / totalTime;
        Vector2 velocity = new Vector2(vx, vy);
        bombRB.velocity = velocity;
    }

    public void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D hit in hits)
        {
            if (!hit.TryGetComponent<BaseStats>(out BaseStats target))
                continue;

            float damage = thrower.CalculateDamageDealt(target, BaseStats.Damage.DamageSource.Normal, out bool crit, out DamagePopup.DamageType damageType);

            if (isParried && target == thrower)
                target.TakeDamage(thrower, new BaseStats.Damage(damage * damageMultToSelf), crit, target.transform.position, damageType);
            else
                target.TakeDamage(thrower, new BaseStats.Damage(damage), crit, target.transform.position, damageType);
        }

        Release();
        gameObject.SetActive(false);
        animator.ResetTrigger("detonate");
        isParried = false;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, targetPos) <= 3f && 
            Physics2D.Raycast(transform.position, Vector2.down, 0.2f + bombCol.size.x, groundLayer))
        {
            animator.SetTrigger("detonate");
            bombCol.isTrigger = false;
        }

        if (transform.position.y > targetPos.y &&
            Physics2D.OverlapCircle(transform.position, bombCol.size.x / 2f) && 
            player.immuneType == BaseStats.ImmuneType.Parry &&
            !isParried)
        {
            isParried = true;
            LaunchBomb(transform.position, thrower.transform.position, 0.4f);
            player.OnParryEnemy(thrower);
        }
    }
}