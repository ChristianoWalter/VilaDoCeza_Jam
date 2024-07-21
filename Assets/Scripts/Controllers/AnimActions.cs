using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimActions : MonoBehaviour
{
    public UnityEvent[] action;

    public void Action1()
    {
        action[0].Invoke();
    }

    public void Action2()
    {
        action[1].Invoke();
    }

    public void Action3()
    {
        action[2].Invoke();
    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
