using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is used to hide visuals when the game starts. 
/// This is useful for having debug visuals in the editor but not in the game itself.
/// It is also possible to show them ingame.
/// </summary>
public class DebugVisual : MonoBehaviour
{
	[Tooltip("show this visual inside of the game instead of just the editor")]
	[SerializeField] private bool m_showWhenRunning = false;

	void Start()
	{
		GetComponent<MeshRenderer>().enabled = m_showWhenRunning;
	}
}