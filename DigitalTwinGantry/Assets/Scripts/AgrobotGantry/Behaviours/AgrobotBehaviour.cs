using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The behaviour defines how the gantry moves and when it takes actions.
/// The AgrobotEquipment on the gantry can be used to detect if there are any interactables in reach.
/// The behaviour can then start actions targeting these interactables.
/// The base class keeps the list of ongoing actions up to date.
/// </summary>
public abstract class AgrobotBehaviour
{
	/// <summary>
	/// Actions are created in this order. Later action types are not created if the interactable has an earlier interaction flag.
	/// </summary>
	private static readonly InteractableFlag[] INTERACTION_ORDER = 
		{ InteractableFlag.HARVEST, InteractableFlag.SOW, InteractableFlag.WATER, InteractableFlag.UPROOT };

	protected AgrobotGantry m_gantry;
	private List<AgrobotAction> m_ongoingActions;
	public List<AgrobotAction> OnGoingActions { get => m_ongoingActions; }

	public AgrobotBehaviour()
	{
		m_ongoingActions = new List<AgrobotAction>();
	}

	virtual public void Start(AgrobotGantry agrobotGantry)
	{
		m_gantry = agrobotGantry;
	}

	abstract public void Update(float deltaTime);

	/// <summary>
	/// Cancels all ongoing actions and stops their coroutines
	/// </summary>
	public virtual void Stop()
	{
		while (m_ongoingActions.Count > 0)
		{
			m_ongoingActions[0].Cancel();
			m_gantry.StopCoroutine(m_ongoingActions[0].ExecutingCoroutine);
			m_ongoingActions.RemoveAt(0);
		}
		m_gantry.StopAllCoroutines();
	}

	/// <summary>
	/// Tries to create an appropriate action targeting this interactable.
	/// Return null if the interactable or appropriate tool is busy.
	/// Also returns null if the interactable has no flags matching the INTERACTION_ORDER.
	/// 
	/// This will only try to create an action for the FIRST flag matching the INTERACTION_ORDER.
	/// If there is no tool available with that flag this will return null.
	/// </summary>
	/// <param name="targetInteractable">the interactable the action should target</param>
	/// <returns>an AgrobotAction or null</returns>
	protected AgrobotAction CreateAction(AgrobotInteractable targetInteractable)
	{
		if (targetInteractable.Busy)
		{
			return null;
		}
		foreach (InteractableFlag flag in INTERACTION_ORDER)
		{
			if (targetInteractable.HasFlag(flag))
			{
				AgrobotTool tool = m_gantry.Equipment.GetTool(flag);
				if (tool != null)
				{
					return new AgrobotAction(targetInteractable, tool, flag, this.ActionFinished, m_gantry.Equipment.InteractableModified);
				}
				return null; //do not check for other flags until the earlier ones are cleared
			}
		}
		return null;
	}

	/// <summary>
	/// Starts an action and adds it to the list of ongoing actions. When the action is finished it is removed from the list by a callback.
	/// Actions on interactables that are busy (already have an ongoing action targeting them) get ignored and cause the method to return false.
	/// </summary>
	/// <param name="action">the AgrobotAction to start</param>
	protected void StartAction(AgrobotAction action)
	{
		m_ongoingActions.Add(action);
		Coroutine coroutine = m_gantry.StartCoroutine(action.Start());
		action.ExecutingCoroutine = coroutine;
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