using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelData
    {
        public string level;
        public Material levelBGMaterial;
        public List<GameObject> GOsToActivate = new();
        public List<GameObject> GOsToDeactivate = new();
        public VolumeProfile volume;
    }
    [SerializeField] private Volume globalVolume;
    [SerializeField] private List<LevelData> levelDatas = new();
    [SerializeField] private List<SpriteRenderer> backgrounds = new();

    private void Start()
    {
        LevelData currentData = null;

        foreach (LevelData data in levelDatas)
        {
            if (data.level == GameData.Instance.currentLevel)
            {
                currentData = data;
                break;
            }
        }

        if (currentData == null)
        {
            Debug.LogWarning("Level in level manager not found");
            return;
        }

        if (currentData.levelBGMaterial != null)
        {
            foreach (SpriteRenderer bgs in backgrounds)
                bgs.material = currentData.levelBGMaterial;
        }

        foreach (GameObject go in currentData.GOsToActivate)
            go.SetActive(true);

        foreach (GameObject go in currentData.GOsToDeactivate)
            go.SetActive(false);

        globalVolume.profile = currentData.volume;
    }
}