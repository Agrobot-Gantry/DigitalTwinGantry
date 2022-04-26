using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An action executed by the gantry targeting a single interactable. Think of harvesting a single crop for example.
/// </summary>
abstract public class AgrobotAction
{
    public AgrobotInteractable Target { get { return m_interactable; } }

    protected AgrobotInteractable m_interactable;
    protected AgrobotEquipment m_equipment;

    private delegate void Callback(AgrobotAction action);
    private Callback m_callback;

    public AgrobotAction(AgrobotBehaviour behaviour, AgrobotInteractable target, AgrobotEquipment equipment)
    {
        m_callback = new Callback(behaviour.ActionFinished);
        m_interactable = target;
        m_equipment = equipment;
    }

    /// <summary>
    /// Returns the flags that are relevant tot this action. These flags will be cleared from the target interactable when this action finishes.
    /// </summary>
    /// <returns>the flags that should be cleared when this action finishes</returns>
    abstract public InteractableFlag GetFlags();

    /// <summary>
    /// This coroutine should execute everything that is needed from the action (animations, interactable manipulation, etc.).
    /// This coroutine MUST call Finish() when the action is complete.
    /// The AgrobotBehaviour starts this coroutine from the AgrobotGantry when StartAction() is called.
    /// </summary>
    /// <returns>a coroutine that takes care of the action and calls Finish() when it's done</returns>
    abstract public IEnumerator Start();

    /// <summary>
    /// Clears the flags (only the ones relevant to this action) of the target interactable and lets the equipment account for this change.
    /// Then it tells the behaviour that this action is finished. Subclasses should call this function when the Start() coroutine is finished.
    /// </summary>
    protected void Finish()
    {
        m_interactable.ClearFlag(GetFlags()); 
        //the flag may be cleared but tools only check flags on collision so we need to update them for this specific interactable
        m_equipment.InteractableModified(m_interactable);
        m_interactable.busy = false;//
        m_callback(this);
    }
}
