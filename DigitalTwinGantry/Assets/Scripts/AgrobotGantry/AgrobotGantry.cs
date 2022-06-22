using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for moving the agrobot gantry and is the main class of the agrobot system
/// </summary>
public class AgrobotGantry : MonoBehaviour
{
    public AgrobotEquipment Equipment { get { return m_equipment; } }
    private AgrobotEquipment m_equipment;
    private AgrobotBehaviour m_currentBehaviour;
    public AgrobotBehaviour CurrentBehaviour { get => m_currentBehaviour; }
    private bool m_counterClockwise = false;
    private bool m_firsRowEnterOccured = false;
    private bool m_firsRowExitOccured = false;
    private bool m_isTurning;

    [SerializeField] private ToolReach m_toolReach;
    [Header("harvest")]
    [SerializeField] private AgrobotTool m_harvestTool;
    [SerializeField] private int m_harvestAmount = 1;
    [Header("sow")]
    [SerializeField] private AgrobotTool m_sowTool;
    [SerializeField] private int m_sowAmount = 1;
    [Header("uproot")]
    [SerializeField] private AgrobotTool m_uprootTool;
    [SerializeField] private int m_uprootAmount = 1;
    [Header("irrigation")]
    [SerializeField] private AgrobotTool m_irrigationTool;
    [SerializeField] private int m_irrigationAmount = 1;

    private Vector3 m_toolPos;

    private List<AgrobotTool> m_tools = new List<AgrobotTool>();
    public List<AgrobotTool> Tools { get => m_tools; }

    /// <summary>
    /// Forward-facing movement speed in meters per second. Setting this to a positive value will make the gantry move forwards. 
    /// Negative values will make it move backwards.
    /// </summary>
    public float MovementSpeed { get; set; }
    /// <summary>
    /// Horizontal rotation speed in degrees per second. Setting this to a positive value will make the gantry move right.
    /// Negative values will make it turn left.
    /// </summary>
    public float TurningSpeed { get; set; }

    /// <summary>
    /// Resets the agrobot (call when a new field is generated)
    /// </summary>
    /// <param name="startPosition">The position the agrobot needs to have after the reset</param>
    /// <param name="startRotation">The rotation the agrobot needs to have after the reset</param>
    public void Reset(Vector3 startPosition, Quaternion startRotation)
    {
        gameObject.transform.position = startPosition;
        gameObject.transform.rotation = startRotation;
        m_isTurning = false;
        m_counterClockwise = false;
        m_firsRowEnterOccured = false;
        m_firsRowExitOccured = false;
        foreach(AgrobotTool tool in m_tools)
        {
            tool.Reset();
        }
        SetBehaviour(new LaneFarmingBehaviour());

    }
    
    /// <summary>
    /// Adds x amount of the specified tool to the agrobot
    /// </summary>
    /// <param name="tool">The tool type that needs to be added</param>
    /// <param name="amount">The amount of the specified tool need to be added</param>
    public void AddTool(AgrobotTool tool, int amount)
    {
        if(amount > 7)
        {
            amount = 7;
            Debug.LogWarning("Amount of arms can't be more then 7");
        }
        float sidePosOffset = 0.2f;
        m_toolPos.y = gameObject.transform.position.y + tool.transform.position.y;
        m_toolPos.z = gameObject.transform.position.z + tool.transform.position.z;
        m_toolPos.x = gameObject.transform.position.x + tool.transform.position.x;
        for ( int i = 1; i<=amount; i++)
        {
            if(i != 1)
            {
                m_toolPos.x += sidePosOffset;
                sidePosOffset *= -1.5f;
            }
            
            AgrobotTool workTool = Instantiate(tool, m_toolPos, Quaternion.identity, gameObject.transform);
            workTool.Reach = m_toolReach;
            m_tools.Add(workTool);
            
        }
    }

    void Start()
    {
        MovementSpeed = 0.0f;
        TurningSpeed = 0.0f;
        AddTool(m_harvestTool, m_harvestAmount);
        AddTool(m_sowTool, m_sowAmount);
        AddTool(m_uprootTool, m_uprootAmount);
        AddTool(m_irrigationTool, m_irrigationAmount);
        m_equipment = new AgrobotEquipment(m_tools.ToArray());
        m_currentBehaviour = new LaneFarmingBehaviour();
        ShowCasing(true);

        m_currentBehaviour.Start(this);
    }

    void Update()
    {
        if (m_tools.Count > 0)
        {
            //moving
            if (MovementSpeed != 0.0f)
            {
                transform.Translate(Vector3.forward * TimeChanger.DeltaTime * MovementSpeed);
                m_isTurning = false;
            }

            //turning
            if (TurningSpeed != 0.0f)
            {
                transform.Rotate(Vector3.up, TimeChanger.DeltaTime * TurningSpeed);
                m_isTurning = true;
            }

            m_currentBehaviour.Update(TimeChanger.DeltaTime);
        }
    }

    public void ShowCasing(bool showCasing)
    {
        //TODO show/hide all pieces of the casing
    }

    public void SetBehaviour(AgrobotBehaviour behaviour)
    {
        if (m_currentBehaviour != null)
        {
            m_currentBehaviour.Stop();
        }
        
        m_currentBehaviour = behaviour;
        m_currentBehaviour.Start(this);
    }

    /// <returns>the total width of the gantry (including the wheels) in meters</returns>
    public float GetGantryWidth()
    {
        return 3.0f;
    }

    /// <returns>the width of the wheels</returns>
    public float GetGantryWheelWidth()
    {
        return 0.5f;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "path" && m_firsRowEnterOccured && !m_isTurning)
        {
            m_firsRowExitOccured = true;
            SetBehaviour(new TurningBehaviour(m_counterClockwise, 1));
        }
       
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "path" && m_firsRowEnterOccured && m_firsRowExitOccured && !m_isTurning)
        {
            SetBehaviour(new TurningBehaviour(m_counterClockwise, 2));
            m_counterClockwise = !m_counterClockwise;
            m_firsRowEnterOccured = false;
            m_firsRowExitOccured = false;
        }
        else if(other.tag == "path" && !m_isTurning)
        {
            m_firsRowEnterOccured = true;
        }       
    }
}