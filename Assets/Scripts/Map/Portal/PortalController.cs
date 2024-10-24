using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    [SerializeField] public List<GameObject> buttons;
    [SerializeField] public List<Portal> portals = new List<Portal>();
    private int baseSize = 70;
    private float multiplier = 1.425f;
    private float decrease = 0.175f;
    private float decreaseDiff = 0.175f;
    private float decreaseMultiplier = 0.005f;

    public void PositionPortals(List<Portal> mapPortals, Camera cam)
    {
        portals.AddRange(mapPortals);

        int x = (int)(cam.orthographicSize - baseSize) / 10;
        float d = decrease;
        if (x > 0)
            multiplier -= d;
        for (int i = 0; i < x - 1; i++)
        {
            float y = Mathf.Clamp(decreaseDiff - (i * decreaseMultiplier), 0, 1);
            d -= y;
            multiplier -= d;
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].transform.localPosition = portals[i].transform.position * multiplier;
            portals[i].button = buttons[i];
            //buttons[i].SetActive(false);
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
