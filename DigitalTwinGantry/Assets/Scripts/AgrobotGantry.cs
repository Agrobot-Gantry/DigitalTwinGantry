using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for moving the agrobot gantry.
/// </summary>
public class AgrobotGantry : MonoBehaviour
{
	private AgrobotBehaviour m_currentBehaviour;

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

	public void SetBehaviour(AgrobotBehaviour behaviour)
    {
		m_currentBehaviour = behaviour;
    }
}