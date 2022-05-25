using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class RosListeningBehaviour : AgrobotBehaviour
{
	public RosListeningBehaviour() : base()
	{

	}

	public override void Update(float deltaTime)
	{
		
	}

	public void Forward()
	{
		Debug.Log("called forward");
	}

	public void ForwardAtSpeed(float speed)
	{

	}
}