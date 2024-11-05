using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgroundManager : MonoBehaviour
{
    private Transform player;
    [SerializeField] private float maxUndegroundY;
    [SerializeField] private List<Animator> backgrounds = new();
    [SerializeField] private List<Animator> caveBackgrounds = new();

    private void Start()
    {
        player = PlayerController.Instance.transform;
    }

    private void Update()
    {
        if (player.position.y <= maxUndegroundY)
        {
            foreach (Animator background in backgrounds)
                background.SetBool("fadeOut", true);
            foreach (Animator caveBackground in caveBackgrounds)
                caveBackground.SetBool("fadeOut", false);
        }
        else
        {
            foreach (Animator background in backgrounds)
                background.SetBool("fadeOut", false);
            foreach (Animator caveBackground in caveBackgrounds)
                caveBackground.SetBool("fadeOut", true);
        }
    }
}