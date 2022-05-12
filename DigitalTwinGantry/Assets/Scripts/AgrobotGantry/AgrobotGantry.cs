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
            transform.Translate(Vector3.forward * Time.deltaTime * MovementSpeed);
        }

        //turning
        if (TurningSpeed != 0.0f)
        {
            transform.Rotate(Vector3.up, Time.deltaTime * TurningSpeed);
        }

        m_currentBehaviour.Update(Time.deltaTime);
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
        return 3.0f; //TODO implement
    }

    /// <returns>the width of the wheels</returns>
    public float GetGantryWheelWidth()
    {
        return 0.5f; //TODO implement
    }
}