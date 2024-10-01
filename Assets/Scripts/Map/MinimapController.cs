using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    [SerializeField] List<GameObject> maps;
    [SerializeField] List<GameObject> cam;
    [SerializeField] PortalController portalController;

    public void ChangeView(int i, int j, bool k)
    {
        cam[i].SetActive(true);
        maps[i].SetActive(true);

        cam[j].SetActive(false);
        maps[j].SetActive(false);

        portalController.gameObject.SetActive(k);
    }
}
