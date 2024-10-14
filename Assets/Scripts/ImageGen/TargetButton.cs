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
    public UnityEvent OnTakeDamage;

    public override bool TakeDamage(BaseStats attacker, Damage damage, bool isCrit, Vector3 closestPoint, DamagePopup.DamageType damageType)
    {
        buttonAnimator.SetTrigger("down");

        switch (buttonType)
        { 
            case ButtonType.StartButton:
                if (bgManager.StartBGGeneration())
                    OnTakeDamage?.Invoke();
                break;
            case ButtonType.ResetButton:
                buttonController.ResetPrompts();
                break;
        }

        return true;
    }
}