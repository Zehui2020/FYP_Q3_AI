using DesignPatterns.ObjectPool;
using UnityEngine;

public class AbilityAnimation : MonoBehaviour
{
    public void DestroyObj()
    {
        Destroy(gameObject);
    }

    public void SpawnAfterEffect()
    {
        //PlayerAfterimage afterimage = ObjectPool.Instance.GetPooledObject("Afterimage", true) as PlayerAfterimage;
        //afterimage.SetupImage(GetComponent<SpriteRenderer>().sprite, transform.localScale.x);
        //afterimage.transform.position = transform.position;
        //afterimage.transform.rotation = transform.rotation;
    }
}