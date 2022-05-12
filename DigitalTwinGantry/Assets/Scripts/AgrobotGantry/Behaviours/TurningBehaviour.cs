using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurningBehaviour : AgrobotBehaviour
{
    private float m_startRotation;
    private Vector3 m_startPosition;
    private float m_rotationTarget = 90.0f;
    private float m_rotationMargin = 0.5f;
    private float m_turningSpeed;

    private bool turnCorrected;
    private int clockwise = 1;
    private int turnHalf;

    public TurningBehaviour(bool counterClockwise, int turnHalf) : base()
    {
        m_turningSpeed = 10.0f;
        this.turnHalf = turnHalf;
        if (counterClockwise)
        {
            m_turningSpeed *= -1.0f;
            clockwise = -1;
        }
    }

    public override void Start(AgrobotGantry agrobotGantry)
    {
        base.Start(agrobotGantry);
        //first half starts with turning, second half with riding
        if (turnHalf == 1)
        {
            m_gantry.MovementSpeed = 0.0f;
            m_gantry.TurningSpeed = m_turningSpeed;
        }
        else if (turnHalf == 2)
        {
            m_gantry.MovementSpeed = 1.0f;
            m_gantry.TurningSpeed = 0.0f;
        }
        m_startRotation = m_gantry.transform.rotation.eulerAngles.y;
        m_startPosition = m_gantry.transform.position;
        turnCorrected = false;
    }

    public override void Update(float deltaTime)
    {
        //if the second half of the turn is called, go the half length of the gantry further
        if (turnHalf == 2 && Vector3.Distance(m_gantry.transform.position, m_startPosition) >= m_gantry.GetGantryWidth()/1.6)
        {
            m_gantry.MovementSpeed = 0.0f;
            m_gantry.TurningSpeed = m_turningSpeed;
        }
        //check if the rotation is more then 90 degrees
        if (CompareAngle(m_startRotation, m_gantry.transform.rotation.eulerAngles.y) >= m_rotationTarget - m_rotationMargin)
        {
            //correct the turn to excactly 90 degrees
            if (!turnCorrected)
            {
                m_gantry.transform.rotation = Quaternion.Euler(0.0f, m_startRotation + (m_rotationTarget * clockwise), 0.0f);
                turnCorrected = true;
            }
            //if its the first half of the turn just go forward, if its the second half go back to farming
            if (turnHalf == 1)
            {
                m_gantry.MovementSpeed = 1.0f;
                m_gantry.TurningSpeed = 0.0f;
            }
            if (turnHalf == 2)
            {
                Debug.Log("Going back to farming again");
                m_gantry.SetBehaviour(new LaneFarmingBehaviour());
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
