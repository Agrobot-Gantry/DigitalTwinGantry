using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for moving the agrobot gantry.
/// </summary>
public class AgrobotGantry : MonoBehaviour
{
    public AgrobotEquipment Equipment { get { return m_equipment; } }
    private AgrobotEquipment m_equipment;
    public AgrobotBehaviour CurrentBehaviour { get => m_currentBehaviour; }
    private AgrobotBehaviour m_currentBehaviour;
    public Transform ResetPosition { get { return m_resetPosition; } set { m_resetPosition = value; } }
    private Transform m_resetPosition;

    private bool m_counterClockwise = false;
    private bool m_firsRowEnterOccured = false;
    private bool m_firsRowExitOccured = false;
    private bool m_isTurning;

    [SerializeField]
    private ToolReach toolReach;
    [Header("harvest")]
    [SerializeField] private AgrobotTool harvestTool;
    [SerializeField] private int harvestAmount = 1;
    [Header("sow")]
    [SerializeField] private AgrobotTool sowTool;
    [SerializeField] private int sowAmount = 1;
    [Header("uproot")]
    [SerializeField] private AgrobotTool uprootTool;
    [SerializeField] private int uprootAmount = 1;
    [Header("irrigation")]
    [SerializeField] private AgrobotTool irrigationTool;
    [SerializeField] private int irrigationAmount = 1;

    private Vector3 toolPos;


    private List<AgrobotTool> m_tools = new List<AgrobotTool>();
    public List<AgrobotTool> tools { get => m_tools; }

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

    public void Reset()
    {
        gameObject.transform.position = m_resetPosition.position;
        gameObject.transform.rotation = m_resetPosition.rotation;
        m_isTurning = false;
        m_counterClockwise = false;
        m_firsRowEnterOccured = false;
        m_firsRowExitOccured = false;
        foreach(AgrobotTool tool in m_tools)
        {
            tool.NewField();
        }
        SetBehaviour(new LaneFarmingBehaviour());

        RosCommandListener rosListener = GetComponent<RosCommandListener>();
        if (rosListener != null) rosListener.StopListening();

    }

    public void addTool(AgrobotTool tool, int amount)
    {
        if(amount > 7)
        {
            amount = 7;
            Debug.LogWarning("Amount of arms can't be more then 7");
        }
        float sidePosOffset = 0.2f;
        toolPos.y = gameObject.transform.position.y + tool.transform.position.y;
        toolPos.z = gameObject.transform.position.z + tool.transform.position.z;
        toolPos.x = gameObject.transform.position.x + tool.transform.position.x;
        for ( int i = 1; i<=amount; i++)
        {
            if(i != 1)
            {
                toolPos.x += sidePosOffset;
                sidePosOffset *= -1.5f;
            }
            
            AgrobotTool workTool = Instantiate(tool, toolPos, Quaternion.identity, gameObject.transform);
            workTool.reach = toolReach;
            m_tools.Add(workTool);
            
        }
    }

	void Awake()
	{
        m_resetPosition = gameObject.transform;
	}

	void Start()
    {
        MovementSpeed = 0.0f;
        TurningSpeed = 0.0f;
        addTool(harvestTool, harvestAmount);
        addTool(sowTool, sowAmount);
        addTool(uprootTool, uprootAmount);
        addTool(irrigationTool, irrigationAmount);
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
        Debug.Log(m_currentBehaviour + " => " + behaviour);
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
        if (m_currentBehaviour.GetType() == typeof(RosListeningBehaviour)) return; //ignore turning triggers when listening to ROS

        if (other.tag == "path" && m_firsRowEnterOccured && !m_isTurning)
        {
            m_firsRowExitOccured = true;
            SetBehaviour(new TurningBehaviour(m_counterClockwise, 1));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_currentBehaviour.GetType() == typeof(RosListeningBehaviour)) return; //ignore turning triggers when listening to ROS

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