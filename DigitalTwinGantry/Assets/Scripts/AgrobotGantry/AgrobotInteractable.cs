using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Flags are to decide which actions should be used for a specific interactable.
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

/// <summary>
/// Interactables can be added to a GameObject so that an AgrobotAction can target it using actions.
/// Setting flags on the interactable dictates what actions will be used on it. Multiple flags can be set on the same interactable.
/// </summary>
public class AgrobotInteractable : MonoBehaviour
{
	public InteractableFlag Flags { get { return m_flags; } }
	[SerializeField] private InteractableFlag m_flags;
	[SerializeField] private GameObject m_interactableObject;
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