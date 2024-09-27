using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    [SerializeField] private WFC_MapGeneration mapGen;

    public List<Portal> portals;



    public void OnTeleport(int i)
    {
        if (portals[i] == null || !portals[i].isActivated)
            return;

        // teleport player
        PlayerController.Instance.transform.position = portals[i].transform.position;
    }
}
