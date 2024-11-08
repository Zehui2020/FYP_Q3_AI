using DesignPatterns.ObjectPool;
using System.Collections;
using UnityEngine;

public class ShadowHand : PooledObject
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float lifetime;
    [SerializeField] private Rigidbody2D handRB;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sr;

    private bool canUpdate = false;

    public void InitShadowHand()
    {
        StartCoroutine(ReleaseRoutine());
    }

    public void StartChasing()
    {
        canUpdate = true;
        StartCoroutine(AfterEffect());
    }

    private void Update()
    {
        if (!canUpdate)
            return;

        Vector2 dirToPlayer = (PlayerController.Instance.transform.position - transform.position).normalized;
        handRB.velocity = dirToPlayer * moveSpeed;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -Mathf.Sign(dirToPlayer.x), transform.localScale.y, transform.localScale.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<PlayerController>(out PlayerController player))
            return;

        player.ChangeState(PlayerController.PlayerStates.ShadowBound);
        Disappear();
    }

    private IEnumerator ReleaseRoutine()
    {
        yield return new WaitForSeconds(lifetime);

        Disappear();
    }

    public void Disappear()
    {
        animator.SetTrigger("disappear");
    }

    public void ReleaseHand()
    {
        Release();
        gameObject.SetActive(false);
        canUpdate = false;
    }

    private IEnumerator AfterEffect()
    {
        while (true)
        {
            PlayerAfterimage afterImage = ObjectPool.Instance.GetPooledObject("Afterimage", true) as PlayerAfterimage;
            afterImage.SetupImage(sr.sprite, transform.localScale.x);
            afterImage.transform.position = transform.position;

            yield return new WaitForSeconds(0.3f);
        }
    }
}