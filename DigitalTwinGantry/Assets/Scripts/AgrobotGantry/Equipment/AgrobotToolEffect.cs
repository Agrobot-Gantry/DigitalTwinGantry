using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AgrobotToolEffect : MonoBehaviour
{
    [SerializeField] private UnityEvent m_onEffectStart;

    public void EffectStart()
    {
        m_onEffectStart.Invoke();
    }
}
