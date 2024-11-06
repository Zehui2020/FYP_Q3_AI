using DesignPatterns.ObjectPool;
using System.Collections;
using UnityEngine;

public class ShadowHand : PooledObject
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float lifetime;
    [SerializeField] private Rigidbody2D handRB;

    public void InitShadowHand()
    {
        StartCoroutine(ReleaseRoutine());
    }

    private void Update()
    {
        Vector2 dirToPlayer = (PlayerController.Instance.transform.position - transform.position).normalized;
        handRB.velocity = dirToPlayer * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<PlayerController>(out PlayerController player))
            return;

        player.ChangeState(PlayerController.PlayerStates.ShadowBound);
        Release();
        gameObject.SetActive(false);
    }

    private IEnumerator ReleaseRoutine()
    {
        yield return new WaitForSeconds(lifetime);

        Release();
        gameObject.SetActive(false);
    }
}