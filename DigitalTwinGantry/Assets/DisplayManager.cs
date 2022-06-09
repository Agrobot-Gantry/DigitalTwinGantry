using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayManager : MonoBehaviour
{
    [SerializeField]
    private GameObject desktop;
    [SerializeField]
    private GameObject[] XRComponents;
    // Start is called before the first frame update
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
        foreach (GameObject XR in XRComponents)
        {
            XR.SetActive(true);
        }
        desktop.SetActive(false);
#endif

    }
}
