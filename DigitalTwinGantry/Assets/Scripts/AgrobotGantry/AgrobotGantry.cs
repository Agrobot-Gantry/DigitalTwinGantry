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
    private bool counterClockwise = false;
    private bool firsRowEnterOccured = false;
    private bool firsRowExitOccured = false;
    private bool isTurning;

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
            isTurning = false;
        }

        //turning
        if (TurningSpeed != 0.0f)
        {
            transform.Rotate(Vector3.up, TimeChanger.DeltaTime * TurningSpeed);
            isTurning = true;
        }

        m_currentBehaviour.Update(TimeChanger.DeltaTime);
    }

    public void ShowCasing(bool showCasing)
    {
        //TODO show/hide all pieces of the casing
    }

    public void SetBehaviour(AgrobotBehaviour behaviour)
    {
        m_currentBehaviour.Stop();
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
        if (other.tag == "path" && firsRowEnterOccured && !isTurning)
        {
            firsRowExitOccured = true;
            SetBehaviour(new TurningBehaviour(counterClockwise, 1));
        }
       
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "path" && firsRowEnterOccured && firsRowExitOccured && !isTurning)
        {
            SetBehaviour(new TurningBehaviour(counterClockwise, 2));
            counterClockwise = !counterClockwise;
            firsRowEnterOccured = false;
            firsRowExitOccured = false;
        }
        else if(other.tag == "path" && !isTurning)
        {
            firsRowEnterOccured = true;
        }
       
    }
}