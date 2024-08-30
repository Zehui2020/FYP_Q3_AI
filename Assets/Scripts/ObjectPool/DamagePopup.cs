using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DesignPatterns.ObjectPool;

public class DamagePopup : PooledObject
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private float driftDistance;

    private Animator animator;

    public enum DamageType
    {
        Normal,
        Crit
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetupPopup(int damage, Vector3 position, DamageType damageType)
    {
        transform.position = position;
        transform.forward = Camera.main.transform.forward;

        switch (damageType)
        {
            case DamageType.Normal:
                textMesh.color = Color.yellow;
                textMesh.SetText(damage.ToString());
                break;
            case DamageType.Crit:
                textMesh.color = Color.red;
                textMesh.SetText(damage.ToString() + "!");
                break;
        }

        Deactivate();
    }

    public void SetupPopup(string text, Vector3 position, Color color)
    {
        transform.position = position;
        transform.forward = Camera.main.transform.forward;

        textMesh.color = color;
        textMesh.SetText(text);

        Deactivate();
    }

    public void Deactivate()
    {
        StartCoroutine(DeactivateRoutine());
    }

    private IEnumerator DeactivateRoutine()
    {
        float timer = 0;
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;

        Vector3 initialPosition = transform.position;
        Vector3 randomDirection = Random.onUnitSphere;
        if (randomDirection.y < 0)
            randomDirection.y *= -1;

        randomDirection.Normalize();
        Vector3 targetPosition = initialPosition + randomDirection * driftDistance;

        while (timer < animationLength)
        {
            float t = timer / animationLength;
            transform.position = Vector3.Lerp(initialPosition, targetPosition, t);

            timer += Time.deltaTime;

            yield return null;
        }

        Release();
        gameObject.SetActive(false);
    }
}