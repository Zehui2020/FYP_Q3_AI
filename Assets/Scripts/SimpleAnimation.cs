using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleAnimation : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Image image;

    [SerializeField] private List<Sprite> sprites = new();
    [SerializeField] private float interval;
    [SerializeField] private bool loop;
    [SerializeField] private bool showOnStart;

    private Coroutine loopRoutine;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        image = GetComponent<Image>();

        if (!showOnStart)
        {
            Hide();
            return;
        }
    }

    private void OnEnable()
    {
        if (loop)
            loopRoutine = StartCoroutine(LoopRoutine());
    }

    public void Hide()
    {
        StopAnimation();

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
        else if (image != null)
            image.enabled = false;
    }

    public void Show()
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;
        else if (image != null)
            image.enabled = true;

        ResumeAnimation();
    }

    public void StopAnimation()
    {
        if (loopRoutine != null)
            StopCoroutine(loopRoutine);

        loopRoutine = null;
    }

    public void ResumeAnimation()
    {
        StopAnimation();
        if (loop && isActiveAndEnabled)
            loopRoutine = StartCoroutine(LoopRoutine());
    }

    private IEnumerator LoopRoutine()
    {
        int counter = 0;

        while (true)
        {
            if (spriteRenderer != null)
                spriteRenderer.sprite = sprites[counter];
            else if (image != null)
                image.sprite = sprites[counter];

            yield return new WaitForSeconds(interval);

            counter++;
            if (counter >= sprites.Count)
                counter = 0;
        }
    }
}