using UnityEngine;

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

	public void Forward()
	{
		Debug.Log("called forward");
	}

	public void Backward()
	{
		Debug.Log("called backward");
	}

	public void ForwardAtSpeed(float speed)
	{

	}

	public void ResetGantry()
	{
		//TODO reset gantry and go to a normal behaviour
	}
}