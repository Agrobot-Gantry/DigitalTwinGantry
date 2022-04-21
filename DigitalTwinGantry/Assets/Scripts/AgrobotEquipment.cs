using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// The equipment contains all of the tools that interact with AgrobotInteractables.
/// AgrobotActions should use this class to get access to the tools they require.
/// </summary>
public class AgrobotEquipment
{
    private AgrobotTool[] m_tools;

    public AgrobotEquipment()
    {
        m_tools = new AgrobotTool[] { }; //TODO get the tools somehow

        //check if each flag has not more than 1 tool assigned
        foreach (InteractableFlag flag in System.Enum.GetValues(typeof(InteractableFlag)))
        {
            int flaggedTools = 0;
            foreach (AgrobotTool tool in m_tools)
            {
                if (tool.GetFlag() == flag)
                {
                    flaggedTools++;
                }
            }
            Assert.IsTrue(flaggedTools <= 1); //never more than 1 tool per flag
        }
    }

    /// <param name="flag">the flag of the desired tool</param>
    /// <returns>the tool that is supposed to manipulate interactables of the specified flag</returns>
    public AgrobotTool GetTool(InteractableFlag flag)
    {
        foreach (AgrobotTool tool in m_tools)
        {
            if (tool.GetFlag() == flag)
            {
                return tool;
            }
        }
        return null;
    }

    /// <returns>all interactables in reach of all tools</returns>
    public AgrobotInteractable[] GetReachables()
    {
        List<AgrobotInteractable> interactables = new List<AgrobotInteractable>();
        foreach (AgrobotTool tool in m_tools)
        {
            interactables.AddRange(tool.Reachables);
        }
        return interactables.ToArray();
    }

    /// <param name="flag">the flag of the desired tool</param>
    /// <returns>all interactables in reach of the tool of the specified flag</returns>
    public AgrobotInteractable[] GetReachables(InteractableFlag flag)
    {
        return GetTool(flag).Reachables.ToArray();
    }
}
