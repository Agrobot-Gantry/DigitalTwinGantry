using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for moving the agrobot gantry.
/// </summary>
public class AgrobotGantry : MonoBehaviour
{
	private IAgrobotBehaviour m_currentBehaviour;
	private Dictionary<AgrobotAction, IAgrobotBehaviour> m_actions;

	/// <summary>
	/// Forward-facing movement speed in meters per second. Setting this to a positive value will make the gantry move forwards. 
	/// Negative values will make it move backwards.
	/// </summary>
	private float MovementSpeed { get; set; }
	/// <summary>
	/// Horizontal rotation speed in degrees per second. Setting this to a positive value will make the gantry move right.
	/// Negative values will make it turn left.
	/// </summary>
	private float TurningSpeed { get; set; }

	void Start()
	{
		ShowCasing(true);

		MovementSpeed = 1.0f;
		TurningSpeed = 10.0f;

		m_actions = new Dictionary<AgrobotAction, IAgrobotBehaviour>();
		m_actions.Add(AgrobotAction.HARVESTING, new AgrobotHarvesting());
		m_actions.Add(AgrobotAction.SOWING, new AgrobotSowing());
		m_actions.Add(AgrobotAction.WEEDING, new AgrobotWeeding());
	}

	void Update()
	{
		//moving
		if (MovementSpeed != 0.0f)
		{
			transform.Translate(transform.forward * Time.deltaTime * MovementSpeed);
		}

		//turning
		if (TurningSpeed != 0.0f)
		{
			transform.Rotate(Vector3.up, Time.deltaTime * TurningSpeed);
		}
	}

	public void ShowCasing(bool showCasing)
    {
		//show/hide all pieces of the casing
    }

	public void DoAction(float actionSpeed)
	{
		m_currentBehaviour.DoAction();
	}

	public void SetAction(AgrobotAction action)
	{
		m_currentBehaviour = m_actions[action];
	}
}

/// <summary>
/// Used to identify AgrobotBehaviours
/// </summary>
public enum AgrobotAction
{
	HARVESTING,
	SOWING,
	WEEDING
}