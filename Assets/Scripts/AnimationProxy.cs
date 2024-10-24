using UnityEngine;
using UnityEngine.Events;

public class AnimationProxy : MonoBehaviour
{
    public UnityEvent OnAnimationEvent;

    public void TriggerEvent()
    {
        OnAnimationEvent?.Invoke();
    }
}