using UnityEngine;

public class BGLoader : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] bgSRs;
    [SerializeField] private ImageSaver imageSaver;

    // Start is called before the first frame update
    void Start()
    {
        Sprite bgImage = imageSaver.GetSpriteFromLocalDisk("background");
        foreach (SpriteRenderer sr in bgSRs)
            sr.sprite = bgImage;
    }
}