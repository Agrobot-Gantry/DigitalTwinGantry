using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class AgrobotGantry : MonoBehaviour
{
    [Tooltip("If the casing should be shown")]
    [SerializeField] private bool showCasing = true;
    private IAgrobotBehaviour m_currentBehaviour;
    private Dictionary<AgrobotAction, IAgrobotBehaviour> m_actions;

    //movement
    private Transform m_transform;
    private float m_speed;
    private float m_turnSpeed;

    // Start is called before the first frame update
    void Start()
    {
        m_transform = GetComponent<Transform>();

        m_actions = new Dictionary<AgrobotAction, IAgrobotBehaviour>();
        m_actions.Add(AgrobotAction.HARVESTING, new AgrobotHarvesting());
        m_actions.Add(AgrobotAction.SOWING, new AgrobotSowing());
        m_actions.Add(AgrobotAction.WEEDING, new AgrobotWeeding());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Drive(float speed)
    {

    }

    public void Stop()
    {

    }

    public void TurnLeft(float turnSpeed)
    {

    }

    public void TurnRight(float turnSpeed)
    {

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