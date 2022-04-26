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
        m_gantry.MovementSpeed = 1.0f;
        m_gantry.TurningSpeed = 0.0f;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        if (this.m_gantry.Equipment.GetReachables().Length > 0)
        {
            //stop and start action
            //continue driving when all have finished
            m_gantry.MovementSpeed = 0.0f;
            StartAction(new HarvestAction(this, this.m_gantry.Equipment.GetReachables()[0], this.m_gantry.Equipment));
        }
    }

    public override void Stop()
    {
        throw new System.NotImplementedException();
    }
}
