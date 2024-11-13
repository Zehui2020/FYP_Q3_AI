using System.Collections;
using UnityEngine;

public class Undead : Enemy
{
    public enum State
    {
        Patrol,
        Chase,
        Attack,
        Rest,
        Teleport,
        Hurt,
        Die
    }
    public State currentState;

    private readonly int RestAnim = Animator.StringToHash("UndeadRest");
    private readonly int WalkAnim = Animator.StringToHash("UndeadWalk");
    private readonly int ChaseAnim = Animator.StringToHash("UndeadChase");
    private readonly int AttackAnim = Animator.StringToHash("UndeadAttack");
    private readonly int TeleportAnim = Animator.StringToHash("UndeadTeleport");
    private readonly int HurtAnim = Animator.StringToHash("UndeadHurt");
    private readonly int DieAnim = Animator.StringToHash("UndeadDie");

    [Header("Undead stats")]
    [SerializeField] private float teleportThreshold;
    [SerializeField] private float teleportCooldown;

    [SerializeField] private float attackMovementSpeed;
    [SerializeField] private float attackDuration;

    private bool canTeleport = true;
    private bool canAttack = true;
    private Coroutine attackRoutine;

    public override void InitializeEnemy()
    {
        base.InitializeEnemy();
        aiNavigation.StopNavigationUntilResume();

        ChangeState(State.Patrol);

        onFinishIdle += () => { ChangeState(State.Patrol); };
        onPlayerInChaseRange += () => { ChangeState(State.Chase); };
        OnBreached += (multiplier) => { if (health <= 0) return; animator.Play(RestAnim); };
        OnParry += (stat) => { if (health <= 0) return; animator.Play(RestAnim); };
        onHitEvent += (target, damage, crit, pos) => { if (CheckHurt()) ChangeState(State.Hurt); };

        audioProxy.PlayAudioOneShot(Sound.SoundName.UndeadLoop);
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

        switch (currentState)
        {
            case State.Patrol:
                animator.Play(WalkAnim, -1, 0);
                break;
            case State.Chase:
                enemyRB.velocity = Vector2.zero;
                animator.Play(ChaseAnim, -1, 0);
                break;
            case State.Attack:
                uiController.ShowAlertSignal();
                animator.Play(AttackAnim, -1, 0);
                attackRoutine = StartCoroutine(AttackRoutine());
                StartCoroutine(AttackCooldown());
                break;
            case State.Rest:
                animator.Play(RestAnim, -1, 0);
                break;
            case State.Teleport:
                animator.Play(TeleportAnim, -1, 0);
                StartCoroutine(TeleportCooldown(0));
                uiController.FadeCanvas(false, 0.4f);

                break;
            case State.Hurt:
                animator.Play(HurtAnim, -1, 0);
                break;
            case State.Die:
                animator.Play(DieAnim, -1, 0);
                break;
        }
    }

    public override void UpdateEnemy()
    {
        base.UpdateEnemy();

        switch (currentState)
        {
            case State.Patrol:
                if (CheckChasePlayer())
                    return;

                transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypoint].position, Time.deltaTime * patrolMovementSpeed);
                transform.localScale = waypoints[currentWaypoint].position.x < transform.position.x ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);

                if (Mathf.Abs(Vector2.Distance(transform.position, waypoints[currentWaypoint].position)) > patrolThreshold)
                    return;

                currentWaypoint++;
                if (currentWaypoint > waypoints.Count - 1)
                    currentWaypoint = 0;

                break;
            case State.Chase:
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, Time.deltaTime * chaseMovementSpeed);

                if (Vector2.Distance(player.transform.position, transform.position) <= attackRange && canAttack)
                    ChangeState(State.Attack);

                if (Vector2.Distance(player.transform.position, transform.position) >= teleportThreshold && canTeleport)
                    ChangeState(State.Teleport);

                UpdateDirectionToPlayer();
                break;
        }            
    }

    public override void OnGetParried()
    {
        base.OnGetParried();
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
    }

    private IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(0.4f);

        float timer = attackDuration;
        while (timer > 0 && Vector2.Distance(transform.position, player.transform.position) > 0.5f)
        {
            timer -= Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, Time.deltaTime * attackMovementSpeed);
            UpdateDirectionToPlayer();
            yield return null;
        }

        ChangeState(State.Rest);
        attackRoutine = null;
    }

    private IEnumerator TeleportCooldown(float delay)
    {
        canTeleport = false;

        yield return new WaitForSeconds(teleportCooldown + delay);

        canTeleport = true;
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;

        yield return new WaitForSeconds(4f);

        canAttack = true;
    }

    public void OnTeleport()
    {
        transform.position = player.transform.position;
        uiController.FadeCanvas(true, 0.4f);
    }
}