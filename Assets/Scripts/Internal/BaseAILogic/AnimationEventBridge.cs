
using System;
using UnityEngine;

public abstract class AnimationEventBridge<T> : MonoBehaviour where T : struct, IConvertible
{
    public event Action<T> onAnimationEvent;

    // This event is called by the Animator
    private void OnEvent_Animation(string eventName)
    {
        if (onAnimationEvent == null)
            return;

        T animationEvent = eventName.GetIdentifierEnum<T>();
        onAnimationEvent(animationEvent);
    }

    public void TriggerCustomAnimationTrigger(T eventType)
    {
        if (onAnimationEvent == null)
            return;

        onAnimationEvent(eventType);
    }
}