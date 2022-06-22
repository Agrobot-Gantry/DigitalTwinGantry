using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Place this at the end of the cropfield so the application can go to the next time period 
/// </summary>
[RequireComponent(typeof(Collider))]
public class EndZone : MonoBehaviour
{
    private Action m_onTrigger;

    public void setEvent(Action onTrigger)
    {
        m_onTrigger = onTrigger;
    }

    private void OnTriggerEnter(Collider other)
    {
        m_onTrigger.Invoke();
    }
}
