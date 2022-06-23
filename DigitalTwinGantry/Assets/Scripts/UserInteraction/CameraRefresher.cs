using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will take all the camera's put into this script and will only refresh one of them each frame.
/// This is used to increase performance but will lower the refresh rate of the camera's.
/// </summary>
public class CameraRefresher : MonoBehaviour
{
    [SerializeField] private Camera[] m_cameras;

    private void Start()
    {
        for (int i = 0; i < m_cameras.Length; i++)
        {
            m_cameras[i].enabled = false;
        }

        StartCoroutine(RefreshCameras());
    }

    private IEnumerator RefreshCameras()
    {
        while (true) 
        {
            for (int i = 0; i < m_cameras.Length; i++)
            {
                m_cameras[i].enabled = true;
                yield return null;
                m_cameras[i].enabled = false;
            }
        }
    }
}
