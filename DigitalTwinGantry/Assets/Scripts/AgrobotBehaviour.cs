using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public interface IAgrobotBehaviour
{
    void DoAction();
}

public class AgrobotHarvesting : IAgrobotBehaviour
{
    public void DoAction()
    {
        
    }
}

public class AgrobotSowing : IAgrobotBehaviour
{
    public void DoAction()
    {

    }
}

public class AgrobotWeeding : IAgrobotBehaviour
{
    public void DoAction()
    {

    }
}