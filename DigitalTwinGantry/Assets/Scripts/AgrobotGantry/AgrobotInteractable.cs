using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Interactables can be added to a GameObject so that an AgrobotAction can target it using actions.
/// Setting flags on the interactable dictates what actions will be used on it. Multiple flags can be set on the same interactable.
/// </summary>
public class AgrobotInteractable : MonoBehaviour
{
	[SerializeField] private GameObject m_interactableObject;
	[SerializeField] private InteractableFlag m_flags;
	[SerializeField] private UnityEvent<AgrobotAction> m_interactionCallback;

	/// <summary>
	/// Used by actions to indicate if an interactable is already being used.
	/// </summary>
	public bool Busy { get { return m_busy; } set { m_busy = value; } }
	private bool m_busy = false;

	/// <summary>
	/// The GameObject that is interactable.
	/// Actions should manipulate this object and not the interactable itself, because it may be used as a hitbox for a parent object.
	/// The interactable will return its own GameObject if none is set in the editor.
	/// </summary>
	public GameObject InteractableObject { get { return m_interactableObject; } }

	void Start()
	{
		if (m_interactableObject == null)
		{
			m_interactableObject = this.gameObject;
		}
	}

	/// <summary>
	/// Creates actions targeting this interactable. The types of action is determined by the flags. 
	/// The actions come in an array sorted in the order in which they should be executed.
	/// </summary>
	/// <param name="behaviour">the behaviour trying to start an action targeting this interactable</param>
	/// <param name="equipment">the equipment of the agrobot running the behaviour</param>
	/// <returns>an array of appropriate AgrobotActions targeting this interactable</returns>
	public AgrobotAction GetAction(AgrobotBehaviour behaviour, AgrobotEquipment equipment)
	{
		AgrobotAction action = null;
		if (this.HasFlag(InteractableFlag.HARVEST))
		{
			AgrobotTool tool = equipment.GetTool(InteractableFlag.HARVEST);
			if (tool != null) action = new AgrobotAction(this, behaviour, equipment, InteractableFlag.HARVEST);
		}
		else if (this.HasFlag(InteractableFlag.SOW))
		{
			AgrobotTool tool = equipment.GetTool(InteractableFlag.SOW);
			if (tool != null) action = new AgrobotAction(this, behaviour, equipment, InteractableFlag.SOW);
		}
		else if (this.HasFlag(InteractableFlag.WATER))
		{
			AgrobotTool tool = equipment.GetTool(InteractableFlag.WATER);
			if (tool != null) action = new AgrobotAction(this, behaviour, equipment, InteractableFlag.WATER);
		}
		else if (this.HasFlag(InteractableFlag.UPROOT))
		{
			AgrobotTool tool = equipment.GetTool(InteractableFlag.UPROOT);
			if (tool != null) action = new AgrobotAction(this, behaviour, equipment, InteractableFlag.UPROOT);
		}
		return action;
	}

	/*public AgrobotAction GetAction(AgrobotBehaviour behaviour, AgrobotEquipment equipment)
    {
		AgrobotAction[] action = new AgrobotAction[FlagCount(m_flags)];
		if (this.HasFlag(InteractableFlag.HARVEST))
		{
			action = new HarvestAction(this, behaviour, equipment);
		}
		if (this.HasFlag(InteractableFlag.SOW))
		{
			action = new SowAction(this, behaviour, equipment);
		}
		if (this.HasFlag(InteractableFlag.WATER))
		{
			action = new IrrigationAction(this, behaviour, equipment);
		}
		if (this.HasFlag(InteractableFlag.UPROOT))
		{
			action = new UprootAction(this, behaviour, equipment);
		}
		AgrobotTool tool = equipment.GetTool(InteractableFlag.HARVEST);
		if (tool == null)
        {
			return null;
        }
		return ;
    }*/

	public void OnInteract(AgrobotAction action)
	{
		if (m_interactionCallback != null)
		{
			m_interactionCallback.Invoke(action);
		}
	}

	public bool HasFlag(InteractableFlag flag)
	{
		return m_flags.HasFlag(flag);
	}

	public void ClearFlag(InteractableFlag flag)
	{
		m_flags &= ~flag;
	}

	public void SetFlags(InteractableFlag flags)
	{
		m_flags = flags;
	}

	/// <returns>the amount of flags that have been set</returns>
	public static int FlagCount(InteractableFlag flag)
	{
		int count = 0;
		while (flag != 0)
		{
			count++;
			flag &= flag - 1; //peel off least significant bit
		}
		return count;
	}
}

/// <summary>
/// Flags are to decide which actions whould be used for a specific interactable.
/// </summary>
[Flags]
public enum InteractableFlag
{
	NONE = 0,
	HARVEST = 1 << 0,
	SOW = 1 << 1,
	WATER = 1 << 2,
	UPROOT = 1 << 3
}