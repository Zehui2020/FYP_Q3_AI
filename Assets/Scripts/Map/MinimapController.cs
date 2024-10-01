using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    [SerializeField] List<GameObject> maps;
    [SerializeField] List<GameObject> cam;

    public void ChangeView(int i, int j)
    {
        if (cam.Count > i)
            cam[i].SetActive(true);
        if (maps.Count > i)
            maps[i].SetActive(true);

        if (cam.Count > j)
            cam[j].SetActive(false);
        if (maps.Count > j)
            maps[j].SetActive(false);
    }
}
