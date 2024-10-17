using System.Collections;
using UnityEngine;
using UnityEngine.Windows;

public class FinalBossPhase1 : Enemy
{
    public enum State
    {
        Idle,
        Walk,
        SlamAttack,
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

    private int punchCounter;

    // Debugging
    private State previousState;

    public void ChangeState(State newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case State.Idle:
                animator.Play(IdleAnim);
                break;
            case State.Walk:
                animator.Play(WalkAnim);
                break;
            case State.SlamAttack:
                StartCoroutine(SlamAttack());
                break;
            case State.PunchAttack:
                punchCounter++;
                animator.Play(RushAnim);
                break;
        }
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Walk:
                enemyRB.MovePosition(transform.position + GetDirectionToPlayer() * Time.deltaTime * walkSpeed);
                break;
            case State.PunchAttack:
                enemyRB.MovePosition(transform.position + GetDirectionToPlayer() * Time.deltaTime * rushSpeed);
                if (Vector2.Distance(transform.position, player.transform.position) <= 0.1f)
                    ChangeState(State.SlamAttack);
                break;
        }
    }

    public void Decide()
    {

    }

    private IEnumerator SlamAttack()
    {
        animator.Play(JumpAnim);

        yield return new WaitForSeconds(jumpWaitDuration);

        Vector3 targetPos = player.transform.position;

        while (true)
        {
            Vector3 direction = (targetPos - transform.position).normalized;
            Vector3 newPos = transform.position + direction * jumpSpeed * Time.deltaTime;

            enemyRB.MovePosition(newPos);

            RaycastHit2D groundHit = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
            if (groundHit)
            {
                ChangeState(State.Idle);
                yield break;
            }

            yield return null;
        }
    }

    public void OnJump()
    {
        StartCoroutine(JumpRoutine());
    }
    private IEnumerator JumpRoutine()
    {
        float timer = 1.5f;
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
}