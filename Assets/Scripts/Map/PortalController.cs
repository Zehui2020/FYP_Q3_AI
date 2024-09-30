using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortalController : MonoBehaviour
{
    [SerializeField] public List<GameObject> buttons;
    [SerializeField] private Transform buttonParent;
    [SerializeField] public List<Portal> portals = new List<Portal>();
    [SerializeField] private float multiplier = 5;
    [SerializeField] private Vector3 offset;

    public void PositionPortals(List<Portal> mapPortals)
    {
        portals.AddRange(mapPortals);

        buttonParent.localPosition = offset;
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].transform.localPosition = portals[i].transform.position * multiplier;
            portals[i].button = buttons[i].GetComponent<Button>();
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
