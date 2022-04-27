using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeChanger : MonoBehaviour
{
    public void ChangeTimeScale(float value) {
        Time.timeScale = value * 2;
    }
}
