using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    [SerializeField] public List<GameObject> buttons;
    [SerializeField] public GameObject doorIcon;
    public List<Portal> portals = new List<Portal>();

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

    public void PositionPortals(List<Portal> mapPortals, Door door, Camera cam)
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

        for (int i = 0; i < portals.Count; i++)
        {
            buttons[i].transform.localPosition = portals[i].transform.position * multiplier;
            portals[i].button = buttons[i];
        }

        doorIcon.transform.localPosition = door.transform.position * multiplier;
        door.icon = doorIcon;
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
        AudioManager.Instance.PlayOneShot(Sound.SoundName.TeleportStart);
        // turn off map and lock it
        mmController.ChangeView(false, false);
        mmController.viewLocked = true;

        // lock player and fade out
        PlayerController.Instance.ChangeState(PlayerController.PlayerStates.Transition);
        PlayerController.Instance.FadeOut();

        // timer for fade out
        yield return new WaitForSeconds(0.75f);

        // teleport player
        PlayerController.Instance.transform.position = portals[i].transform.position;

        // timer for player teleport
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlayOneShot(Sound.SoundName.TeleportEnd);
        yield return new WaitForSeconds(0.5f);

        // unlock map
        mmController.viewLocked = false;

        // unlock player and fade back in
        PlayerController.Instance.ChangeState(PlayerController.PlayerStates.Movement);
        PlayerController.Instance.FadeIn();
        teleportRoutine = null;
    }

    public void ActivateAllPortals()
    {
        for (int i = 0; i < portals.Count; i++)
        {
            portals[i].OnEnterRange();
        }
    }
}
