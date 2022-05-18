using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EndZone : MonoBehaviour
{
    private UnityEvent m_onTrigger;

    public void setEvent(UnityEvent onTrigger)
    {
        m_onTrigger = onTrigger;
    }

    private void OnTriggerEnter(Collider other)
    {
        m_onTrigger.Invoke();
    }
}
