using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestAction : AgrobotAction
{
    public HarvestAction(AgrobotBehaviour behaviour, AgrobotInteractable target, AgrobotEquipment equipment) : base(behaviour, target, equipment)
    {

    }

    public override InteractableFlag GetFlags()
    {
        return InteractableFlag.HARVEST;
    }

    public override void Start()
    {
        //m_interactable.busy = true;
        Debug.Log("started harvest");//
    }

    public override void Update(float deltaTime)
    {
        Debug.Log("updated harvest");//
        Finish();
    }
}
