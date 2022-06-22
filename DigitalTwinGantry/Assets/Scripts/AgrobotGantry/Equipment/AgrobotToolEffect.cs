using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A simple class with a UnityEvent which is supposed to be used to call tool effects of the agrobot. 
/// </summary>
public class AgrobotToolEffect : MonoBehaviour
{
    [SerializeField] private UnityEvent m_onEffectStart;

    public void EffectStart()
    {
        m_onEffectStart.Invoke();
    }
}
