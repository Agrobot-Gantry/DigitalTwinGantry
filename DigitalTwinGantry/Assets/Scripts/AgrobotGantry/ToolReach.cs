using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the reach of a tool. Anything within the collider is in reach. Multiple tools can make use of the same ToolReach.
/// </summary>
public class ToolReach : MonoBehaviour
{
    [Tooltip("show the reach area visual inside of the game instead of just the editor")]
    public bool showReach = false;
    //TODO make the reach only count interactables if their center is inside?
    //TODO or change interactables to have very small hitboxes

    private delegate void ToolCallback(AgrobotInteractable interactable);
    private List<ToolCallback> m_enterCallbacks;
    private List<ToolCallback> m_exitCallbacks;

    void Awake()
    {
        GetComponent<MeshRenderer>().enabled = showReach; //the reach area should only be visible in the editor
        m_enterCallbacks = new List<ToolCallback>();
        m_exitCallbacks = new List<ToolCallback>();
    }

    public void ConnectToTool(AgrobotTool tool)
    {
        m_enterCallbacks.Add(new ToolCallback(tool.OnReachEnter));
        m_exitCallbacks.Add(new ToolCallback(tool.OnReachExit));
    }

    void OnTriggerEnter(Collider other)
    {
        AgrobotInteractable interactable = other.gameObject.GetComponent<AgrobotInteractable>();
        if (interactable != null)
        {
            foreach (ToolCallback callback in m_enterCallbacks)
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
