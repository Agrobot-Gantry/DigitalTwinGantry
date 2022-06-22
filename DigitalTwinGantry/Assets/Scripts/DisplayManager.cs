using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will turn on and off all the desktop/XR components dependant on the build type (PC version or Android version)
/// </summary>
public class DisplayManager : MonoBehaviour
{
    [SerializeField] private GameObject m_desktop;
    [SerializeField] private GameObject[] m_XRComponents;

    void Start()
    {
#if UNITY_STANDALONE
        desktop.SetActive(true);
        foreach (GameObject XR in XRComponents)
        {
            XR.SetActive(false);
        }
#endif
#if UNITY_ANDROID
        foreach (GameObject XR in m_XRComponents)
        {
            XR.SetActive(true);
        }
        m_desktop.SetActive(false);
#endif
    }
}
