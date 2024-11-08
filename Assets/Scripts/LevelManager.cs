using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelData
    {
        public string level;
        public Material levelBGMaterial;
        public float backgroundScaleX = -1;
        public List<GameObject> GOsToActivate = new();
        public List<GameObject> GOsToDeactivate = new();
        public VolumeProfile volume;
        public float globalLightIntensity;
    }
    [SerializeField] private Volume globalVolume;
    [SerializeField] private Light2D globalLight;
    [SerializeField] private List<LevelData> levelDatas = new();
    [SerializeField] private List<SpriteRenderer> backgrounds = new();

    [Header("For Boss")]
    [SerializeField] private List<ParallaxEffect> parallaxBGs = new();
    [SerializeField] private TilemapManager tilemapManager;

    private LevelData previousRandomLevel;

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

        if (currentData.backgroundScaleX != -1)
            backgrounds[0].transform.localScale = new Vector3(currentData.backgroundScaleX, currentData.backgroundScaleX, currentData.backgroundScaleX);

        foreach (GameObject go in currentData.GOsToActivate)
            go.SetActive(true);

        foreach (GameObject go in currentData.GOsToDeactivate)
            go.SetActive(false);

        globalVolume.profile = currentData.volume;
        globalLight.intensity = currentData.globalLightIntensity;
    }

    public void ChangeRandomTheme()
    {
        LevelData currentData;

        do
        {
            currentData = levelDatas[Random.Range(0, levelDatas.Count - 1)];
        }
        while (currentData == previousRandomLevel);

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

        if (currentData.backgroundScaleX != -1)
            backgrounds[0].transform.localScale = new Vector3(currentData.backgroundScaleX, currentData.backgroundScaleX, currentData.backgroundScaleX);

        //foreach (GameObject go in currentData.GOsToActivate)
        //    go.SetActive(true);

        foreach (GameObject go in currentData.GOsToDeactivate)
            go.SetActive(false);

        foreach (ParallaxEffect bg in parallaxBGs)
            bg.ChangeSprite(currentData.level);

        globalVolume.profile = currentData.volume;
        globalLight.intensity = currentData.globalLightIntensity;

        tilemapManager.SliceTexture(currentData.level);
    }
}