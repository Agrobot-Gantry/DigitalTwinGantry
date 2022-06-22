using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This behaviour makes the gantry move forward until its tools fall behind which makes it wait for them.
/// </summary>
public class LaneFarmingBehaviour : AgrobotBehaviour
{
	public LaneFarmingBehaviour() : base()
	{

	}

	public override void Start(AgrobotGantry agrobotGantry)
	{
		base.Start(agrobotGantry);
		m_gantry.TurningSpeed = 0.0f;
	}

    public override void Update(float deltaTime)
    {
        foreach (AgrobotTool tool in m_gantry.Tools)
        {
            if (tool.GoingTooFast)
            {
                m_gantry.MovementSpeed = 0.0f;
                break;
            }
            m_gantry.MovementSpeed = AgrobotDefinitions.Instance.MovementSpeed; //keep driving
        }
           
            if (this.m_gantry.Equipment.GetReachables().Length > 0) //check reach
            {
                foreach(AgrobotInteractable reachable in m_gantry.Equipment.GetReachables())
                {
                AgrobotAction action = CreateAction(reachable);
                    if(action != null)
                    {
                        StartAction(action);
                    }
                }
            }
    }

}
