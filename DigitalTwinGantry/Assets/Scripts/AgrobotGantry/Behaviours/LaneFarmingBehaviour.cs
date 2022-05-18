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
        if (m_ongoingActions.Count == 0)
        {
            m_gantry.MovementSpeed = 1.0f; //keep driving
            if (this.m_gantry.Equipment.GetReachables().Length > 0) //check reach
            {
                AgrobotAction[] actions = m_gantry.Equipment.GetReachables()[0].GetActions(this, m_gantry.Equipment); //for now we just assume there's just one in reach
                StartAction(actions[0]); //for now we just assume there's just one action
            }
        }
        else
        {
            m_gantry.MovementSpeed = 0.0f; //stop until the action is complete
        }
        //TODO stop moving if there are no valid interactables left
    }


}
