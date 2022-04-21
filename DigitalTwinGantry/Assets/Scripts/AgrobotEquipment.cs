using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AgrobotEquipment
{
    private AgrobotTool[] m_tools; //TODO get the tools somehow

    public AgrobotEquipment()
    {
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

    public AgrobotTool GetTool(InteractableFlag flag)
    {
        foreach (AgrobotTool tool in m_tools)
        {
            if(tool.GetFlag() == flag)
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
        foreach(AgrobotTool tool in m_tools)
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
