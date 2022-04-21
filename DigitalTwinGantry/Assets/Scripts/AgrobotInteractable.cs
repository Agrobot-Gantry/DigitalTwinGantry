using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interactables can be added to a crop so that an AgrobotAction can target that crop.
/// </summary>
public class AgrobotInteractable : MonoBehaviour
{
    [SerializeField] InteractableFlag m_flags;

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

    public void ClearFlag(InteractableFlag flag)
    {
        m_flags &= ~flag;
    }
}

/// <summary>
/// Flags are to decide which actions whould be used for a specific interactable.
/// </summary>
[Flags]
public enum InteractableFlag
{
    NONE = 0,
    SOW = 1 << 0,
    WATER = 1<<1,
    HARVEST = 1 << 2,
    UPROOT = 1 << 3
}