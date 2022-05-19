using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgrobotTurnZone : MonoBehaviour
{
    public bool counterClockwise = false;

    void OnTriggerEnter(Collider other)
    {
        AgrobotGantry gantry = other.gameObject.GetComponent<AgrobotGantry>();
        if (gantry != null)
        {
            gantry.SetBehaviour(new TurningBehaviour(counterClockwise,0));
        }
    }
}
