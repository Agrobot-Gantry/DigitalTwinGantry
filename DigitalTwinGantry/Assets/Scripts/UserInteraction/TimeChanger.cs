using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeChanger : MonoBehaviour
{
    private static float m_timeScale = 1;

    public static float DeltaTime {
        get => (Time.deltaTime * m_timeScale);
    }

    public void ChangeTimeScale(float value) {
        //Time.timeScale = value * 3;
        m_timeScale = value * 3;
    }
}
