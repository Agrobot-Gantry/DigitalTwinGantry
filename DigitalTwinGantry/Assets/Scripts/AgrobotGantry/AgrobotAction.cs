using System.Collections;
using UnityEngine;

/// <summary>
/// An action executed by the gantry targeting a single interactable. Think of harvesting a single crop for example.
/// </summary>
public class AgrobotAction
{
    private AgrobotInteractable m_targetInteractable;
	public AgrobotInteractable TargetInteractable { get { return m_targetInteractable; } }

    private AgrobotEquipment m_equipment;

	private delegate void Callback(AgrobotAction action);
	private Callback m_callback;

    private AgrobotTool m_tool;

    private InteractableFlag m_flags;
	public InteractableFlag Flags => m_flags;

    public AgrobotAction(AgrobotInteractable target, AgrobotBehaviour behaviour, AgrobotEquipment equipment, InteractableFlag flags)
	{
		m_callback = new Callback(behaviour.ActionFinished);
		m_targetInteractable = target;
		m_equipment = equipment;
        m_flags = flags;
		m_tool = equipment.GetTool(m_flags);
	}

    /// <summary>
	/// This coroutine should execute everything that is needed from the action (animations, interactable manipulation, etc.).
	/// This coroutine MUST call Finish() when the action is complete.
	/// The AgrobotBehaviour starts this coroutine from the AgrobotGantry when StartAction() is called.
	/// </summary>
	/// <returns>a coroutine that takes care of the action and calls Finish() when it's done</returns>
	public IEnumerator Start()
	{
        m_tool.busy = true;

		Vector3 target = new Vector3(m_targetInteractable.transform.position.x,
		          m_tool.GetToolObject().transform.position.y, m_tool.GetToolObject().transform.position.z);
		
		while (m_targetInteractable != null && Vector3.Distance(m_tool.GetToolObject().transform.position, target) > 0.1f)
	    {
            target = new Vector3(m_targetInteractable.transform.position.x,
                m_tool.GetToolObject().transform.position.y, m_tool.GetToolObject().transform.position.z);

			m_tool.GetToolObject().transform.position = Vector3.MoveTowards(
		    m_tool.GetToolObject().transform.position, target,
		    AgrobotDefinitions.Instance.EquipmentSpeed * TimeChanger.DeltaTime);

            yield return null;
	    }

		yield return m_tool.PickupInteractable(m_targetInteractable, m_flags, AgrobotDefinitions.Instance.EquipmentSpeed);

        if (m_targetInteractable != null)
        {
            m_targetInteractable.OnInteract(this);
        }
        else
        {
            Debug.Log("TargetInteractable is null");
        }

        m_tool.busy = false;
        Finish();
	}

	/// <summary>
	/// Clears the flags (only the ones relevant to this action) of the target interactable and lets the equipment account for this change.
	/// Then it tells the behaviour that this action is finished. Subclasses should call this function when the Start() coroutine is finished.
	/// </summary>
	public void Finish()
	{
		if (m_targetInteractable != null)
		{
			m_targetInteractable.ClearFlag(m_flags);
			m_targetInteractable.Busy = false;
		}
		//the flag may be cleared but tools only check flags on collision so we need to update them for this specific interactable
		m_equipment.InteractableModified(m_targetInteractable);
		m_callback(this);
	}
}
