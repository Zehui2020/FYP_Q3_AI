using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgroundManager : MonoBehaviour
{
    private Transform player;
    [SerializeField] private float maxUndegroundY;
    [SerializeField] private bool canFade = true;

    [SerializeField] private List<ParallaxEffect> backgrounds = new();
    [SerializeField] private List<ParallaxEffect> caveBackgrounds = new();


    private void Start()
    {
        player = PlayerController.Instance.transform;

        if (!canFade)
        {
            foreach (ParallaxEffect caveBackground in caveBackgrounds)
                caveBackground.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        foreach (ParallaxEffect background in backgrounds)
            background.UpdateParallax();

        if (!canFade)
            return;

        foreach (ParallaxEffect caveBackground in caveBackgrounds)
            caveBackground.UpdateParallax();

        if (player.position.y <= maxUndegroundY)
        {
            foreach (ParallaxEffect background in backgrounds)
                background.Fade(true);
            foreach (ParallaxEffect caveBackground in caveBackgrounds)
                caveBackground.Fade(false);
        }
        else
        {
            foreach (ParallaxEffect background in backgrounds)
                background.Fade(false);
            foreach (ParallaxEffect caveBackground in caveBackgrounds)
                caveBackground.Fade(true);
        }
    }
}