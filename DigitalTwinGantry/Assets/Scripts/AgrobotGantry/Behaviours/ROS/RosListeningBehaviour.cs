using UnityEngine;

/// <summary>
/// Contains methods that the RosCommandListener can call to move the gantry.
/// </summary>
class RosListeningBehaviour : AgrobotBehaviour
{
	public RosListeningBehaviour() : base()
	{

	}

	public override void Start(AgrobotGantry agrobotGantry)
	{
		base.Start(agrobotGantry);
		m_gantry.MovementSpeed = 0.0f;
		m_gantry.TurningSpeed = 0.0f;
	}

	public override void Update(float deltaTime)
	{
		
	}

	public void MoveForward()
	{
		m_gantry.MovementSpeed = AgrobotDefinitions.Instance.MovementSpeed;
	}

	public void MoveBackward()
	{
		m_gantry.MovementSpeed = -AgrobotDefinitions.Instance.MovementSpeed;
	}

	public void TurnLeft()
	{
		m_gantry.TurningSpeed = -AgrobotDefinitions.Instance.TurningSpeed;
	}

	public void TurnRight()
	{
		m_gantry.TurningSpeed = AgrobotDefinitions.Instance.TurningSpeed;
	}

	public void StopMoving()
	{
		m_gantry.MovementSpeed = 0.0f;
		m_gantry.TurningSpeed = 0.0f;
	}

}