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
    protected AgrobotEquipment m_equipment;

    public AgrobotAction(AgrobotBehaviour behaviour, AgrobotInteractable target, AgrobotEquipment equipment)
    {
        m_callback = new Callback(behaviour.ActionFinished);
        m_interactable = target;
        m_equipment = equipment;
    }

    abstract public void Start();

    abstract public void Update(float deltaTime);

    /// <summary>
    /// Returns the flags that are relevant tot this action. These flags will be cleared from the target interactable when this action finishes.
    /// </summary>
    /// <returns>the flags that should be cleared when this action finishes</returns>
    abstract public InteractableFlag GetFlags();

    /// <summary>
    /// Clears the flags (the ones relevant to this action) of the target interactable and tells the behaviour that this action is finished. 
    /// Subclasses should call this function when they are finished.
    /// </summary>
    protected void Finish()
    {
        m_interactable.ClearFlag(GetFlags());
        m_callback(this);
    }
}
