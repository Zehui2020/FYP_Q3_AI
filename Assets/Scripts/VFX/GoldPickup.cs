using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;

public class GoldPickup : PooledObject
{
    private PlayerController player;
    [SerializeField] private Collider2D triggerCol;
    [SerializeField] private Rigidbody2D goldRB;
    [SerializeField] private float launchForceX;
    [SerializeField] private Vector2 launchForceY;

    [SerializeField] private TrailRenderer trail;

    [SerializeField] private float startingChaseSpeed;
    [SerializeField] private float chaseAcceleration;

    private Coroutine chaseRoutine;

    private int goldToGive;

    public override void Init()
    {
        player = PlayerController.Instance;
        trail.enabled = false;
        triggerCol.enabled = false;
        base.Init();
    }

    public void InitGoldPickup(int goldAmount)
    {
        chaseRoutine = StartCoroutine(ChasePlayer());
        goldToGive = goldAmount;
    }

    private IEnumerator ChasePlayer()
    {
        float randLaunchX = Random.Range(-launchForceX, launchForceX);
        float randLaunchY = Random.Range(launchForceY.x, launchForceY.y);
        goldRB.AddForce(new Vector2(randLaunchX, randLaunchY), ForceMode2D.Impulse);

        float randWait = Random.Range(0.5f, 2f);

        yield return new WaitForSeconds(randWait);

        triggerCol.enabled = true;
        trail.enabled = true;
        goldRB.isKinematic = true;

        float speed = startingChaseSpeed;
        float acceleration = chaseAcceleration;

        while (true)
        {
            if (!gameObject.activeInHierarchy)
                yield break;

            speed += acceleration * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

            yield return null;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (chaseRoutine != null)
                StopCoroutine(chaseRoutine);

            player.gold += goldToGive;

            chaseRoutine = null;
            trail.enabled = false;
            goldRB.isKinematic = false;
            triggerCol.enabled = false;
            goldToGive = 0;

            Release();
            gameObject.SetActive(false);
        }
    }
}