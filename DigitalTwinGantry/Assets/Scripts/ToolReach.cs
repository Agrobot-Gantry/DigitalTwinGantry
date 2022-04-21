using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// have it support multiple tools
/// </summary>
public class ToolReach : MonoBehaviour
{
    private delegate void ToolCallback(AgrobotInteractable interactable);
    private List<ToolCallback> m_enterCallbacks;
    private List<ToolCallback> m_exitCallback;

    void Awake()
    {
        m_enterCallbacks = new List<ToolCallback>();
        m_exitCallback = new List<ToolCallback>();
    }

    public void ConnectToTool(AgrobotTool tool)
    {
        m_enterCallbacks.Add(new ToolCallback(tool.OnReachEnter));
        m_enterCallbacks.Add(new ToolCallback(tool.OnReachExit));
    }

    void OnTriggerEnter(Collider other)
    {
        AgrobotInteractable interactable = other.gameObject.GetComponent<AgrobotInteractable>();
        if (interactable != null)
        {
            foreach(ToolCallback callback in m_enterCallbacks)
            {
                callback(interactable);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        AgrobotInteractable interactable = other.gameObject.GetComponent<AgrobotInteractable>();
        if (interactable != null)
        {
            foreach (ToolCallback callback in m_exitCallbacks)
            {
                callback(interactable);
            }
        }
    }
}
