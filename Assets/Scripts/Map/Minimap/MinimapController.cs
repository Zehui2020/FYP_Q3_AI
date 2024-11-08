using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    [SerializeField] List<GameObject> maps;
    [SerializeField] List<GameObject> cam;
    public bool viewLocked = false;

    private void Start()
    {
        if (cam.Count < 2)
            cam.Add(cam[0]);
        else if (cam[1] == null)
            cam[1] = cam[0];
        ChangeView(false, false);
    }

    public void ChangeView(bool showMap, bool playSound)
    {
        if (viewLocked)
            return;

        if (showMap && playSound)
            AudioManager.Instance.PlayOneShot(Sound.SoundName.Map);
        if (!showMap && playSound)
            AudioManager.Instance.PlayOneShot(Sound.SoundName.MapClose);

        cam[0].SetActive(!showMap);
        maps[0].SetActive(!showMap);
        cam[1].SetActive(showMap);
        maps[1].SetActive(showMap);

        if (cam[0] == cam[1])
            cam[0].SetActive(true);
    }
}
