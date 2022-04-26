using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// The reach of the tool is defined by the collider of the ToolReach object.
/// The tool itself (visuals and animation) is the tool parameter in the editor.
/// The flag parameter defines which type of interactable this tool can interact with.
/// Each tool keeps a list of all interactables (with the right flag) that it can reach.
/// </summary>
public class AgrobotTool : MonoBehaviour
{
    [SerializeField]
    private ToolReach m_reach;
    [SerializeField]
    private GameObject m_tool;
    [SerializeField]
    private InteractableFlag m_flag;
    public List<AgrobotInteractable> Reachables { get { return m_reachables; } }
    private List<AgrobotInteractable> m_reachables;

    void Start()
    {
        m_reachables = new List<AgrobotInteractable>();
        m_reach.ConnectToTool(this);

        //check if the tool has exactly one flag set
        Assert.IsTrue(AgrobotInteractable.FlagCount(m_flag) == 1);
    }

    void Update()
    {

    }

    public void OnReachEnter(AgrobotInteractable interactable)
    {
        if (interactable.HasFlag(m_flag))
        {
            m_reachables.Add(interactable);
        }
        //TODO remove interactables that are still in reach but no longer have the necessary flag
    }

    public void OnReachExit(AgrobotInteractable interactable)
    {
        if (interactable.HasFlag(m_flag)) //TODO probably remove this check
        {
            m_reachables.Remove(interactable);
        }
    }

    /// <summary>
    /// Removes an interactable from the list of reachables if it does not have the appropriate flag.
    /// This is needed because interactable flags will be removed while the interactable is still in reach,
    /// and the tool otherwise only checks for flags when the interactable enters/exits its reach.
    /// </summary>
    /// <param name="interactable">the interactable that was modified</param>
    public void InteractableModified(AgrobotInteractable interactable)
    {
        if (!interactable.HasFlag(m_flag))
        {
            m_reachables.Remove(interactable);
        }
    }

    /// <returns>the GameObject that represents the visuals and animation of the tool</returns>
    public GameObject GetToolObject()
    {
        return m_tool;
    }

    public InteractableFlag GetFlag()
    {
        return m_flag;
    }
}
