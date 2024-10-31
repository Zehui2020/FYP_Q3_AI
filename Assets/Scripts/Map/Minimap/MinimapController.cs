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
        ChangeView(false);
    }

    public void ChangeView(bool showMap)
    {
        if (viewLocked)
            return;

        cam[0].SetActive(!showMap);
        cam[1].SetActive(showMap);
        maps[0].SetActive(!showMap);
        maps[1].SetActive(showMap);
    }
}
