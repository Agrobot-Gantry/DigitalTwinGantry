using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple example of a behaviour. Makes the gantry move forward intil it runs into something it's tools can reach.
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
        foreach (AgrobotTool tool in m_gantry.tools)
        {
            if (tool.goingTooFast)
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
                    AgrobotAction action = reachable.GetAction(this, m_gantry.Equipment);
                    if(action != null && !reachable.Busy)
                    {
                        StartAction(action);//for now we just assume there's just one action
                    }
                }
                //AgrobotAction action = m_gantry.Equipment.GetReachables()[0].GetActions(this, m_gantry.Equipment); //for now we just assume there's just one in reach
            }
    }

    public override void Stop()
    {
        base.Stop();
        while(m_ongoingActions.Count > 0)
        {
            m_ongoingActions[0].Finish();
        }
       
    }

}
