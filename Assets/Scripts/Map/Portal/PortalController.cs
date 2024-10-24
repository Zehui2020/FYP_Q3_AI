using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    [SerializeField] public List<GameObject> buttons;
    [SerializeField] public List<Portal> portals = new List<Portal>();
    [SerializeField] private float multiplier = 5;

    public void PositionPortals(List<Portal> mapPortals, Camera cam)
    {
        portals.AddRange(mapPortals);

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].transform.localPosition = portals[i].transform.position * multiplier;
            portals[i].button = buttons[i];
            buttons[i].SetActive(false);
        }
        gameObject.SetActive(false);
    }

    public void OnTeleport(int i)
    {
        if (!portals[i].isActivated)
            return;

        // teleport player
        PlayerController.Instance.transform.position = portals[i].transform.position;
    }
}
