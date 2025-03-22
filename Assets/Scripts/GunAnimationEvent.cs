using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : UnityEvent<string>
{ 

}

public class GunAnimationEvent : MonoBehaviour
{
    public AnimationEvent AnimationEvent = new();

    public void OnAnimationEvent(string eventName)
    {
        AnimationEvent.Invoke(eventName);
    }
}
