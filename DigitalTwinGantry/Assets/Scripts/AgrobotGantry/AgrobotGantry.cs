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
    private AgrobotBehaviour m_currentBehaviour;
    public AgrobotBehaviour CurrentBehaviour { get => m_currentBehaviour; }
    private bool m_counterClockwise = false;
    private bool m_firsRowEnterOccured = false;
    private bool m_firsRowExitOccured = false;
    private bool m_isTurning;

    [SerializeField]
    private AgrobotTool[] m_tools;

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

    public void Reset(Vector3 startPosition, Quaternion startRotation)
    {
        gameObject.transform.position = startPosition;
        gameObject.transform.rotation = startRotation;
        m_isTurning = false;
        m_counterClockwise = false;
        m_firsRowEnterOccured = false;
        m_firsRowExitOccured = false;
        SetBehaviour(new LaneFarmingBehaviour());

    }

    void Start()
    {
        MovementSpeed = 0.0f;
        TurningSpeed = 0.0f;
        m_equipment = new AgrobotEquipment(m_tools);
        m_currentBehaviour = new LaneFarmingBehaviour();
        ShowCasing(true);

        m_currentBehaviour.Start(this);
    }

    void Update()
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