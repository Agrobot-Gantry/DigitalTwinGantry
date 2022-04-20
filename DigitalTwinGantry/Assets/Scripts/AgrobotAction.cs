using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An action executed by the gantry targeting a single interactable. Think of harvesting a single crop for example.
/// </summary>
abstract public class AgrobotAction
{
    private delegate void Callback(AgrobotAction action);
    private Callback m_callback;

    protected AgrobotInteractable m_interactable;

    public AgrobotAction(AgrobotBehaviour behaviour, AgrobotInteractable target)
    {
        m_callback = new Callback(behaviour.ActionFinished);
        m_interactable = target;
    }

    abstract public void Start();

    /// <summary>
    /// Tells the behaviour that this action is finished. Subclasses should call this function when they are finished.
    /// </summary>
    protected void Finish()
    {
        m_callback(this);
    }
}
