using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This behaviour makes the agrobot turn 90 degrees, then drive half of the length of the agrobot forward, and finally turns 90 degrees again.
/// </summary>
public class TurningBehaviour : AgrobotBehaviour
{
    private float m_startRotation;
    private Vector3 m_startPosition;
    private float m_rotationTarget = 90.0f;
    private float m_rotationMargin = 0.5f;
    private float m_turningSpeed;

    private bool m_turnCorrected;
    private int m_clockwise = 1;
    private int m_turnHalf;

    public TurningBehaviour(bool counterClockwise, int turnHalf) : base()
    {
        m_turningSpeed = 1;
        m_turnHalf = turnHalf;
        if (counterClockwise)
        {
            m_turningSpeed *= -1.0f;
            m_clockwise = -1;
        }
    }

    public override void Start(AgrobotGantry agrobotGantry)
    {
        base.Start(agrobotGantry);
        //first half starts with turning, second half with riding
        if (m_turnHalf == 1)
        {
            m_gantry.MovementSpeed = 0.0f;
            m_gantry.TurningSpeed = AgrobotDefinitions.Instance.TurningSpeed * m_turningSpeed;
        }
        else if (m_turnHalf == 2)
        {
            m_gantry.MovementSpeed = AgrobotDefinitions.Instance.MovementSpeed;
            m_gantry.TurningSpeed = 0.0f;
        }
        m_startRotation = m_gantry.transform.rotation.eulerAngles.y;
        m_startPosition = m_gantry.transform.position;
        m_turnCorrected = false;
    }

    public override void Update(float deltaTime)
    {
        // if the second half of the turn is called, go the half length of the gantry further
        if (m_turnHalf == 2 && Vector3.Distance(m_gantry.transform.position, m_startPosition) >= (m_gantry.GetGantryWidth()/2)+(m_gantry.GetGantryWheelWidth() * 1.5))
        {
            m_gantry.MovementSpeed = 0.0f;
            m_gantry.TurningSpeed = AgrobotDefinitions.Instance.TurningSpeed * m_turningSpeed;
        }
        // check if the rotation is more then 90 degrees
        if (CompareAngle(m_startRotation, m_gantry.transform.rotation.eulerAngles.y) >= m_rotationTarget - m_rotationMargin)
        {
            // correct the turn to excactly 90 degrees
            if (!m_turnCorrected)
            {
                m_gantry.transform.rotation = Quaternion.Euler(0.0f, m_startRotation + (m_rotationTarget * m_clockwise), 0.0f);
                m_turnCorrected = true;
            }
            // if its the first half of the turn just go forward, if its the second half go back to farming
            if (m_turnHalf == 1)
            {
                m_gantry.MovementSpeed = AgrobotDefinitions.Instance.MovementSpeed;
                m_gantry.TurningSpeed = 0.0f;
            }
            if (m_turnHalf == 2)
            {
                m_gantry.SetBehaviour(new LaneFarmingBehaviour());
            }
        }
    }

    /// <summary>
    /// Calculates the difference between two angles (absolute value)
    /// </summary>
    /// <param name="angle1">First angle</param>
    /// <param name="angle2">Second angle</param>
    /// <returns>The difference between the angles (absolute value)</returns>
    private float CompareAngle(float angle1, float angle2)
    {
        return 180 - System.Math.Abs(System.Math.Abs(angle1 - angle2) - 180);
    }
}
