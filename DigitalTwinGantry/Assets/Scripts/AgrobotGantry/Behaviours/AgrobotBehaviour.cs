using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The behaviour defines how the gantry moves and when it takes actions.
/// The AgrobotEquipment on the gantry can be used to detect if there are any interactables in reach.
/// The behaviour can then start actions targeting these interactables.
/// The base class keeps the list of ongoing actions up to date.
/// </summary>
abstract public class AgrobotBehaviour
{
    protected AgrobotGantry m_gantry;
    protected List<AgrobotAction> m_ongoingActions;
    protected AgrobotInteractable[] m_allInteractables;

    public AgrobotBehaviour()
    {
        m_ongoingActions = new List<AgrobotAction>();
        m_allInteractables = Object.FindObjectsOfType<AgrobotInteractable>();
    }

    virtual public void Start(AgrobotGantry agrobotGantry)
    {
        m_gantry = agrobotGantry;
    }

    abstract public void Update(float deltaTime);

    public void Stop()
    {
       m_gantry.StopAllCoroutines();
    }

    /// <summary>
    /// Updates the list of interactables this behaviour is keeping track of (which should be all of them).
    /// Also updates the interactables for the equipment. Call this when interactables have been added or removed.
    /// </summary>
    public void UpdateAllInteractables()
    {
        m_allInteractables = Object.FindObjectsOfType<AgrobotInteractable>();
        //TODO update equipment
        foreach (AgrobotInteractable interactable in m_allInteractables)
        {
            m_gantry.Equipment.InteractableModified(interactable);
        }
    }

    /// <summary>
    /// Starts an action and adds it to the list of ongoing actions. When the action is finished it is removed from the list by a callback.
    /// Actions on interactables that are busy (already have an ongoing action targeting them) get ignored and cause the method to return false.
    /// </summary>
    /// <param name="action">the AgrobotAction to start</param>
    /// <returns>true if the action could be started</returns>
    protected bool StartAction(AgrobotAction action)
    {        
        if (action.TargetInteractable == null || action.TargetInteractable.Busy)
        {
            Debug.LogWarning("tried to start an action on an interactable that was already busy");
            return false;
        }
        action.TargetInteractable.Busy = true;
        m_ongoingActions.Add(action);
        m_gantry.StartCoroutine(action.Start());
        return true;
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