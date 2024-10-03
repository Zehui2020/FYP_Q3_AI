using System.Collections;
using UnityEngine;
using TMPro;
using DesignPatterns.ObjectPool;

public class DamagePopup : PooledObject
{
    [SerializeField] private TextMeshProUGUI textMesh;

    [ColorUsageAttribute(false, true)] [SerializeField] private Color critDamageColor;
    [ColorUsageAttribute(false, true)] [SerializeField] private Color shieldDamageColor;
    [ColorUsageAttribute(false, true)] [SerializeField] private Color healthDamageColor;

    private Vector2 driftDir;

    private Animator animator;

    public enum DamageType
    {
        Shield,
        Health,
        Crit,
        OnHitProcc
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetupPopup(int damage, Vector3 position, DamageType damageType, Vector2 driftDirection)
    {
        transform.position = position;
        transform.forward = Camera.main.transform.forward;
        driftDir = driftDirection;

        switch (damageType)
        {
            case DamageType.Shield:
                textMesh.color = shieldDamageColor;
                textMesh.SetText(damage.ToString());
                break;
            case DamageType.Health:
                textMesh.color = healthDamageColor;
                textMesh.SetText(damage.ToString());
                break;
            case DamageType.Crit:
            case DamageType.OnHitProcc:
                textMesh.color = critDamageColor;
                textMesh.SetText(damage.ToString() + "!");
                break;
        }

        Deactivate();
    }

    public void SetupPopup(string text, Vector3 position, Color color, Vector2 driftDirection)
    {
        transform.position = position;
        transform.forward = Camera.main.transform.forward;
        driftDir = driftDirection;

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

        Vector3 targetPosition = initialPosition + new Vector3(driftDir.x, driftDir.y, 0);

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