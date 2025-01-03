using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TargetButton : Enemy
{
    public enum ButtonType
    {
        StartButton,
        ResetButton
    }

    public ButtonType buttonType;
    [SerializeField] private ComfyBGManager bgManager;
    [SerializeField] private WorldSpaceButtonController buttonController;
    [SerializeField] private Animator buttonAnimator;

    public override bool TakeDamage(BaseStats attacker, Damage damage, bool isCrit, Vector3 closestPoint, DamagePopup.DamageType damageType)
    {
        if (damage.damage <= 0)
            return false;

        buttonAnimator.SetTrigger("down");

        AudioManager.Instance.PlayOneShot(Sound.SoundName.GenerationButton);

        switch (buttonType)
        { 
            case ButtonType.StartButton:
                bgManager.StartBGGeneration();
                OnTakeDamage?.Invoke();
                break;
            case ButtonType.ResetButton:
                buttonController.ResetPrompts();
                break;
        }

        return true;
    }
}