using UnityEngine;

public class BGLoader : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] bgSRs;
    [SerializeField] private ImageSaver imageSaver;
    [SerializeField] private string fileName;

    // Start is called before the first frame update
    void Start()
    {
        Sprite bgImage = imageSaver.GetSpriteFromLocalDisk(fileName + "_Level" + GameData.Instance.currentLevel);
        foreach (SpriteRenderer sr in bgSRs)
            sr.sprite = bgImage;
    }
}