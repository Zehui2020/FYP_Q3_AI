using UnityEngine;

public class TutorialGuide : Interactable
{
    private PlayerController player;

    private void Start()
    {
        player = PlayerController.Instance;
    }

    public override bool OnInteract()
    {
        player.DisablePlayer(true);

        return true;
    }
}