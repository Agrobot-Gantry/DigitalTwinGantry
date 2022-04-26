using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Agrobot goes forward and moves harvestables as soon as it reaches them
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
        //TODO add callback from equipment to notify if new valid interactables are in reach
        if (m_ongoingActions.Count == 0)
        {
            m_gantry.MovementSpeed = 1.0f; //keep driving
            if (this.m_gantry.Equipment.GetReachables().Length > 0) //check reach
            {
                StartAction(new HarvestAction(this, this.m_gantry.Equipment.GetReachables()[0], this.m_gantry.Equipment));
            }
        }
        else
        {
            m_gantry.MovementSpeed = 0.0f; //stop until the action is complete
        }
        
    }

    public override void Stop()
    {
        
    }
}
