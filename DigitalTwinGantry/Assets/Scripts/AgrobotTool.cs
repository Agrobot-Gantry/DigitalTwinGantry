using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// This script should be added to an invisible cube that defines the reach of the tool. 
/// The tool itself (visuals and animation) is the tool parameter in the editor.
/// The flag parameter defines which type of interactable this tool can interact with.
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
    }

    public void OnReachExit(AgrobotInteractable interactable)
    {
        if (interactable.HasFlag(m_flag))
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
