using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

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
