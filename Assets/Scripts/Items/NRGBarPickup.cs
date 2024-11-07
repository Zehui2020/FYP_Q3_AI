using DesignPatterns.ObjectPool;
using System.Collections;
using UnityEngine;

public class NRGBarPickup : PooledObject
{
    private SpriteRenderer spriteRenderer;
    private ImageSaver imageSaver;
    private ObjectBobbing objectBobbing;

    [SerializeField] private float lifetime;

    public override void Init()
    {
        base.Init();
        spriteRenderer = GetComponent<SpriteRenderer>();
        imageSaver = GetComponent<ImageSaver>();
        objectBobbing = GetComponent<ObjectBobbing>();

        spriteRenderer.sprite = imageSaver.GetSpriteFromLocalDisk(Item.ItemType.NRGBar.ToString());
        objectBobbing.InitBobbing();
    }

    private void OnEnable()
    {
        if (objectBobbing == null)
            return;

        objectBobbing.InitBobbing();
        StartCoroutine(ReleaseRoutine());
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.TryGetComponent<AbilityController>(out AbilityController abilityController))
            return;

        abilityController.ResetAbilityCooldowns();
        AudioManager.Instance.PlayOneShot(Sound.SoundName.NRGBar);

        Release();
        gameObject.SetActive(false);
    }

    private IEnumerator ReleaseRoutine()
    {
        yield return new WaitForSeconds(lifetime);

        if (!isActiveAndEnabled)
            yield break;

        Release();
        gameObject.SetActive(false);
    }
}