using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    [SerializeField] public List<GameObject> buttons;
    [SerializeField] public List<Portal> portals = new List<Portal>();

    private MinimapController mmController;
    private Coroutine teleportRoutine;
    
    private int baseSize = 70;
    private float multiplier = 1.425f;
    private float decrease = 0.175f;
    private float decreaseDiff = 0.035f;
    private float decreaseMultiplier = 0.005f;

    private void Start()
    {
        for (int i = 0; i < buttons.Count; i++)
            buttons[i].SetActive(false);
    }

    public void PositionPortals(List<Portal> mapPortals, Camera cam)
    {
        mmController = GetComponent<MinimapController>();
        portals.AddRange(mapPortals);

        int x = (int)(cam.orthographicSize - baseSize) / 10;
        float d = decrease;
        if (x > 0)
            multiplier -= d;
        for (int i = 0; i < x - 1; i++)
        {
            float y = Mathf.Clamp(decreaseDiff - (i * decreaseMultiplier), 0, Mathf.Infinity);
            d -= y;
            multiplier = Mathf.Clamp(multiplier - d, 0, Mathf.Infinity);
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].transform.localPosition = portals[i].transform.position * multiplier;
            portals[i].button = buttons[i];
        }
    }

    public void OnTeleport(int i)
    {
        if (!portals[i].isActivated)
            return;

        if (teleportRoutine == null)
            teleportRoutine = StartCoroutine(TeleportRoutine(i));
    }

    private IEnumerator TeleportRoutine(int i)
    {
        // turn off map and lock it
        mmController.ChangeView(false);
        mmController.viewLocked = true;

        // lock player and fade out
        PlayerController.Instance.ChangeState(PlayerController.PlayerStates.Transition);
        PlayerController.Instance.FadeOut();

        // timer for fade out
        yield return new WaitForSeconds(0.75f);

        // teleport player
        PlayerController.Instance.transform.position = portals[i].transform.position;

        // timer for player teleport
        yield return new WaitForSeconds(0.75f);

        // unlock map
        mmController.viewLocked = false;

        // unlock player and fade back in
        PlayerController.Instance.ChangeState(PlayerController.PlayerStates.Movement);
        PlayerController.Instance.FadeIn();
        teleportRoutine = null;
    }
}
