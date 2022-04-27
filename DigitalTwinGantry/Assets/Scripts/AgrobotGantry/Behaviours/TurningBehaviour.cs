using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurningBehaviour : AgrobotBehaviour
{
    private float m_startRotation;
    private Vector3 m_startPosition;
    private float m_rotation1 = 90.0f;
    private float m_rotation2 = 90.0f;
    private float m_rotationMargin = 0.5f;
    private float m_turningSpeed;

    private int step = 0;
    private int clockwise = 1;

    public TurningBehaviour(bool counterClockwise) : base()
    {
        m_turningSpeed = 10.0f;
        if (counterClockwise)
        {
            m_turningSpeed *= -1.0f;
            clockwise = -1;
        }
    }

    public override void Start(AgrobotGantry agrobotGantry)
    {
        base.Start(agrobotGantry);
        m_gantry.MovementSpeed = 0.0f;
        m_gantry.TurningSpeed = m_turningSpeed;
        m_startRotation = m_gantry.transform.rotation.eulerAngles.y;
        m_startPosition = m_gantry.transform.position;
    }

    public override void Update(float deltaTime)
    {
        if (CompareAngle(m_startRotation, m_gantry.transform.rotation.eulerAngles.y) >= m_rotation1 - m_rotationMargin)
        {
            if (step == 0)
            {
                m_gantry.transform.rotation = Quaternion.Euler(0.0f, m_startRotation + (m_rotation1 * clockwise), 0.0f);
                step++;
            }

            //TODO set exactly
            m_gantry.MovementSpeed = 1.0f;
            m_gantry.TurningSpeed = 0.0f;

            if (Vector3.Distance(m_gantry.transform.position, m_startPosition) >= m_gantry.GetGantryWidth())
            {
                m_gantry.MovementSpeed = 0.0f;
                m_gantry.TurningSpeed = m_turningSpeed;

                if (CompareAngle(m_startRotation, m_gantry.transform.rotation.eulerAngles.y) >= m_rotation1 + m_rotation2 - m_rotationMargin)
                {
                    if (step == 1)
                    {
                        m_gantry.transform.rotation = Quaternion.Euler(0.0f, m_startRotation + ((m_rotation1 + m_rotation2) * clockwise), 0.0f);
                        step++;
                    }

                    m_gantry.SetBehaviour(new LaneFarmingBehaviour());
                }
            }
        }
        
    }

    private float CompareAngle(float angle1, float angle2)
    {
        return 180 - System.Math.Abs(System.Math.Abs(angle1 - angle2) - 180);
    }

    public override void Stop()
    {
        
    }
}
