using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interactables can be added to a GameObject so that an AgrobotAction can target it using actions.
/// </summary>
public class AgrobotInteractable : MonoBehaviour
{
    [SerializeField] InteractableFlag m_flags;

    public bool Busy { get { return m_busy; } set { m_busy = value; } }
    private bool m_busy = false;

    /// <summary>
    /// Creates actions targeting this interactable. The types of action is determined by the flags. 
    /// The actions come in an array sorted in the order in which they should be executed.
    /// </summary>
    /// <returns>an array of appropriate AgrobotActions targeting this interactable</returns>
    public AgrobotAction[] GetActions()
    {
        //TODO implement
        return null;
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
        while(flag != 0)
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
    WATER = 1<<2,
    UPROOT = 1 << 3
}