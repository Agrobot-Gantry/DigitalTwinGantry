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
    protected AgrobotInteractable[] m_interactables;

    public AgrobotBehaviour()
    {
        m_ongoingActions = new List<AgrobotAction>();
        m_interactables = Object.FindObjectsOfType<AgrobotInteractable>();
    }

    virtual public void Start(AgrobotGantry agrobotGantry)
    {
        m_gantry = agrobotGantry;
    }

    abstract public void Update(float deltaTime);

    abstract public void Stop();

    /// <summary>
    /// Starts an action and adds it to the list of ongoing actions. When the action is finished it is removed from the list by a callback.
    /// Actions on interactables that are busy (already have an ongoing action targeting them) get ignored and cause the method to return false.
    /// </summary>
    /// <param name="action">the AgrobotAction to start</param>
    /// <returns>true if the action could be started</returns>
    protected bool StartAction(AgrobotAction action)
    {
        if (action.TargetInteractable.Busy)
        {
            return false;
        }
        action.TargetInteractable.Busy = true;
        m_ongoingActions.Add(action);
        m_gantry.StartCoroutine(action.Start());
        return true;
    }

    /// <summary>
    /// Updates the interactables this behaviour is keeping track of (which should be all of them).
    /// Call this when interactables have been added or removed.
    /// </summary>
    public void UpdateInteractables(AgrobotInteractable[] interactables)
    {
        m_interactables = Object.FindObjectsOfType<AgrobotInteractable>();
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