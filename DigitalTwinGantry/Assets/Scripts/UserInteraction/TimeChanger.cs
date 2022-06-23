using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script should be used across the whole application to retrieve the correctly scaled delta time.
/// The orginal Unity delta time is not scaled because player interactions would break because of this.
/// </summary>
public class TimeChanger : MonoBehaviour
{
    [SerializeField] private int m_scaleFactor;

    private static float m_timeScale = 1f;

    public static float DeltaTime => (Time.deltaTime * m_timeScale);
    public static float TimeScale => m_timeScale;

    public void ChangeTimeScale(float value) 
    {
        m_timeScale = value * m_scaleFactor;
    }
}
