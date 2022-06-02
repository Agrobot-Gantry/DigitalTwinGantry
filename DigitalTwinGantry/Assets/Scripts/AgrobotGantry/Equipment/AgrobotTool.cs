using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// The reach of the tool is defined by the collider of the ToolReach object.
/// The tool itself (visuals and animation) is the tool parameter in the editor.
/// The flag parameter defines which type of interactable this tool can interact with.
/// Each tool keeps a list of all interactables (with the right flag) that it can reach.
/// Actions make use of tools for animation.
/// </summary>
public class AgrobotTool : MonoBehaviour
{
    [SerializeField]
    private ToolReach m_reach;
    [SerializeField]
    private GameObject m_tool;
    [SerializeField]
    private InteractableFlag m_flag;

    /// <summary>
    /// A list of all interactables that are in reach of this tool and have an appropriate flag.
    /// </summary>
    public List<AgrobotInteractable> Reachables { get { return m_reachables; } }
    private List<AgrobotInteractable> m_reachables;

    public bool goingTooFast { get { return m_goingTooFast; } }
    private bool m_goingTooFast;
    public bool busy = false;

    void Start()
    {
        m_reachables = new List<AgrobotInteractable>();
        m_reach.ConnectToTool(this);

        //check if the tool has exactly one flag set
        Assert.IsTrue(AgrobotInteractable.FlagCount(m_flag) == 1);
    }
    public void NewField()
    {
        if (m_reachables != null)
        {
            m_reachables.Clear();
        }
        this.busy = false;
    }

    void Update()
    {
        if(m_reachables.Count == 0)
        {
            m_goingTooFast = false;
        }

    }

    public void OnReachEnter(AgrobotInteractable interactable)
    {
        if (interactable.HasFlag(m_flag))
        {
            m_reachables.Add(interactable);
        }
    }

    public void OnReachExit(AgrobotInteractable interactable)
    {
        if (m_reachables.Contains(interactable))
        {
            m_goingTooFast = true;
        }
        
       // m_reachables.Remove(interactable);
    }

    /// <summary>
    /// Removes an interactable from the list of reachables if it does not have the appropriate flag.
    /// This is needed because interactable flags will be removed while the interactable is still in reach,
    /// and the tool otherwise only checks for flags when the interactable enters/exits its reach.
    /// </summary>
    /// <param name="interactable">the interactable that was modified</param>
    public void InteractableModified(AgrobotInteractable interactable)
    {

        if (interactable == null)
        {
            m_reachables.Remove(interactable);
        }
        else if (!interactable.HasFlag(m_flag))
        {
            m_reachables.Remove(interactable);
        }
        for (int i = 0; i < m_reachables.Count; i++)
        {
            if(m_reachables[i] == null)
            {
                m_reachables.RemoveAt(i);
                i--;
            }
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
