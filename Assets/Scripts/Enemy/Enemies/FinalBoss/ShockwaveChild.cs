using UnityEngine;
using UnityEngine.Events;

public class ShockwaveChild : MonoBehaviour
{
    public UnityEvent<Collider2D> OnTriggerEnter;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerEnter?.Invoke(collision);

        if (collision.CompareTag("Player"))
            animator.SetTrigger("end");
    }
}