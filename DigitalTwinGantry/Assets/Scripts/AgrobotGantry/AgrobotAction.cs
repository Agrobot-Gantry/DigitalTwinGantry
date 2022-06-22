using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// An action executed by the gantry targeting a single interactable. Think of harvesting a single crop for example.
/// </summary>
public class AgrobotAction
{
    private AgrobotInteractable m_targetInteractable;
	public AgrobotInteractable TargetInteractable => m_targetInteractable;

	private AgrobotTool m_tool;

	public InteractableFlag Flags => m_flags;
	private InteractableFlag m_flags;

	public Coroutine ExecutingCoroutine { get { return m_coroutine; } set { m_coroutine = value; } }
	private Coroutine m_coroutine;

	private Action<AgrobotAction>[] m_onFinishedcallbacks;

	public AgrobotAction(AgrobotInteractable target, AgrobotTool tool, InteractableFlag flags, params Action<AgrobotAction>[] onFinishedCallbacks)
	{
		m_targetInteractable = target;
		m_targetInteractable.Busy = true; //to prevent the creation of multiple actions with the same interactable
		m_tool = tool;
		m_tool.Busy = true; //to prevent the creation of multiple actions with the same tool
		m_flags = flags;
		m_onFinishedcallbacks = onFinishedCallbacks;
	}

	/// <summary>
	/// This coroutine should execute everything that is needed from the action (animations, interactable manipulation, etc.).
	/// This coroutine MUST call Finish() when the action is complete.
	/// The AgrobotBehaviour starts this coroutine from the AgrobotGantry when StartAction() is called.
	/// </summary>
	/// <returns>a coroutine that takes care of the action and calls Finish() when it's done</returns>
	public IEnumerator Start()
	{
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

		yield return m_tool.Interact(m_targetInteractable, m_flags, AgrobotDefinitions.Instance.EquipmentSpeed);

		if (m_targetInteractable != null)
		{
			m_targetInteractable.OnInteract(this);
		}
		else
		{
			Debug.Log("TargetInteractable is null");
		}

		Finish();
	}

	/// <summary>
	/// Clears the flags (only the ones relevant to this action) of the target interactable and lets the equipment account for this change.
	/// Then it tells the behaviour that this action is finished. Subclasses should call this function when the Start() coroutine is finished.
	/// </summary>
	public void Finish()
	{
		m_tool.Busy = false;
		if (m_targetInteractable != null)
		{
			m_targetInteractable.ClearFlag(m_flags);
			m_targetInteractable.Busy = false;
		}
		
		foreach (Action<AgrobotAction> action in m_onFinishedcallbacks)
		{
			action.Invoke(this);
		}
	}

	public void Cancel()
	{
		m_tool.ActionCancelled();
		if (m_targetInteractable != null)
		{
			m_targetInteractable.Busy = false;
		}
	}
}
