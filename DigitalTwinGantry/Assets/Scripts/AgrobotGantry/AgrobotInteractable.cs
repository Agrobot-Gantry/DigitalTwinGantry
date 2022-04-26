using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interactables can be added to a GameObject so that an AgrobotAction can target it using actions.
/// Setting flags on the interactable dictates what actions will be used on it. Multiple flags can be set on the same interactable.
/// </summary>
public class AgrobotInteractable : MonoBehaviour
{
    [SerializeField] InteractableFlag m_flags;

    /// <summary>
    /// Used by actions to indicate if an interactable is already being used.
    /// </summary>
    public bool Busy { get { return m_busy; } set { m_busy = value; } }
    private bool m_busy = false;

    /// <summary>
    /// Creates actions targeting this interactable. The types of action is determined by the flags. 
    /// The actions come in an array sorted in the order in which they should be executed.
    /// </summary>
    /// <param name="behaviour">the behaviour trying to start an action targeting this interactable</param>
    /// <param name="equipment">the equipment of the agrobot running the behaviour</param>
    /// <returns>an array of appropriate AgrobotActions targeting this interactable</returns>
    public AgrobotAction[] GetActions(AgrobotBehaviour behaviour, AgrobotEquipment equipment)
    {
        int index = 0;
        AgrobotAction[] actions = new AgrobotAction[FlagCount(m_flags)];
        if (this.HasFlag(InteractableFlag.HARVEST))
        {
            actions[index] = new HarvestAction(this, behaviour, equipment);
            index++;
        }
        if (this.HasFlag(InteractableFlag.SOW))
        {
            throw new NotImplementedException("No AgrobotAction has been implemented for this flag!");
        }
        if (this.HasFlag(InteractableFlag.WATER))
        {
            throw new NotImplementedException("No AgrobotAction has been implemented for this flag!");
        }
        if (this.HasFlag(InteractableFlag.UPROOT))
        {
            throw new NotImplementedException("No AgrobotAction has been implemented for this flag!");
        }
        return actions;
    }

    public bool HasFlag(InteractableFlag flag)
    {
        return m_flags.HasFlag(flag);
    }

    public void ClearFlag(InteractableFlag flag)
    {
        m_flags &= ~flag;
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