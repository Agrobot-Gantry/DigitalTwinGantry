using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class AgrobotGantry : MonoBehaviour
{
    [Tooltip("Show the fictional exterior casing")]
    [SerializeField] private bool showCasing = true;
    private IAgrobotBehaviour m_currentBehaviour;
    private Dictionary<AgrobotAction, IAgrobotBehaviour> m_actions;

    /// <summary>
    /// Forward-facing movement speed in meters per second.
    /// </summary>
    private float MovementSpeed { get; set; }
    /// <summary>
    /// Horizontal rotation speed in degrees per second.
    /// </summary>
    private float TurningSpeed { get; set; }

    void Start()
    {
        MovementSpeed = 1.0f;
        TurningSpeed = 10.0f;

        m_actions = new Dictionary<AgrobotAction, IAgrobotBehaviour>();
        m_actions.Add(AgrobotAction.HARVESTING, new AgrobotHarvesting());
        m_actions.Add(AgrobotAction.SOWING, new AgrobotSowing());
        m_actions.Add(AgrobotAction.WEEDING, new AgrobotWeeding());
    }

    void Update()
    {
        //moving
        if(MovementSpeed != 0.0f)
        {
            transform.Translate(transform.forward * Time.deltaTime * MovementSpeed);
        }

        //turning
        if(TurningSpeed != 0.0f)
        {
            transform.Rotate(Vector3.up, Time.deltaTime * TurningSpeed);
        }
    }

    public void DoAction(float actionSpeed)
    {
        m_currentBehaviour.DoAction();
    }

    public void SetAction(AgrobotAction action)
    {
        m_currentBehaviour = m_actions[action];
    }
}

/// <summary>
/// Used to identify AgrobotBehaviours
/// </summary>
public enum AgrobotAction
{
    HARVESTING,
    SOWING,
    WEEDING
}