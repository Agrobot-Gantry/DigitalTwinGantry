using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
abstract public class AgrobotAction
{
    private delegate void Callback(AgrobotAction action);
    private Callback m_callback;

    public AgrobotAction(AgrobotBehaviour behaviour)
    {
        m_callback = new Callback(behaviour.ActionFinished);
    }

    public void Start()
    {

    }

    protected void Finish()
    {
        m_callback(this);
    }
}
