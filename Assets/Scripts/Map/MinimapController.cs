using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    [SerializeField] List<GameObject> maps;
    [SerializeField] List<GameObject> cam;

    public void ChangeView(bool showMap)
    {
        if (showMap)
        {
            if (cam.Count > 1)
            {
                cam[0].SetActive(false);
                cam[1].SetActive(true);
            }
            maps[0].SetActive(false);
            maps[1].SetActive(true);
        }
        else
        {
            if (cam.Count > 1)
            {
                cam[1].SetActive(false);
                cam[0].SetActive(true);
            }
            maps[1].SetActive(false);
            maps[0].SetActive(true);
        }
    }
}
