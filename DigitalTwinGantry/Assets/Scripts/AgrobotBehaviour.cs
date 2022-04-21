using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The behaviour defines how the gantry moves and what actions it takes.
/// The behaviour can start actions targeting nearby interactables for things such as harvesting.
/// The base class keeps the list of ongoing actions up to date.
/// </summary>
abstract public class AgrobotBehaviour
{
    protected AgrobotGantry m_gantry;
    protected List<AgrobotAction> m_ongoingActions;
    protected List<AgrobotInteractable> m_interactables;

    AgrobotBehaviour()
    {
        m_ongoingActions = new List<AgrobotAction>();
        m_interactables = new List<AgrobotInteractable>(); //TODO fill this list
    }

    virtual public void Start(AgrobotGantry agrobotGantry)
    {
        m_gantry = agrobotGantry;
    }

    virtual public void Update(float deltaTime)
    {
        foreach(AgrobotAction action in m_ongoingActions) {
            action.Update(deltaTime);
        }
    }

    abstract public void Stop();

    /// <summary>
    /// Starts an action and adds it to the list of ongoing actions. When the action is finished it is removed from the list by a callback.
    /// </summary>
    /// <param name="action">the AgrobotAction to start</param>
    protected void StartAction(AgrobotAction action)
    {
        m_ongoingActions.Add(action);
    }

    /// <summary>
    /// Callback method for actions to call when they are completed.
    /// </summary>
    /// <param name="action">the completed action</param>
    public void ActionFinished(AgrobotAction action)
    {
        m_ongoingActions.Remove(action);
    }
}