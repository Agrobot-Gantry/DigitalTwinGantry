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
    private GameObject m_tool;
    [SerializeField]
    private InteractableFlag m_flag;
    public List<AgrobotInteractable> Reachables { get; }

    void Start()
    {
        //check if the tool has exactly one flag set
        Assert.IsTrue(AgrobotInteractable.FlagCount(m_flag) == 1);
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        AgrobotInteractable interactable = other.gameObject.GetComponent<AgrobotInteractable>();
        if (interactable != null && interactable.HasFlag(m_flag))
        {
            Reachables.Add(interactable);
        }
    }

    void OnTriggerExit(Collider other)
    {
        AgrobotInteractable interactable = other.gameObject.GetComponent<AgrobotInteractable>();
        if (interactable != null && interactable.HasFlag(m_flag))
        {
            Reachables.Remove(interactable);
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
