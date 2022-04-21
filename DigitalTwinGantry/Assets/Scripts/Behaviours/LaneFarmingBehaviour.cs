using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneFarmingBehaviour : AgrobotBehaviour
{
    public LaneFarmingBehaviour() : base()
    {
        
    }

    public override void Start(AgrobotGantry agrobotGantry)
    {
        base.Start(agrobotGantry);
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        if (this.m_gantry.Equipment.GetReachables().Length > 0)
        {

        }
    }

    public override void Stop()
    {
        throw new System.NotImplementedException();
    }
}
