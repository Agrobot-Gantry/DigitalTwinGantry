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

    public override IEnumerator Start()
    {
        Debug.Log("started harvest");//

        yield return new WaitForSeconds(1.0f);
        Debug.Log("finishing harvest");//
        Finish();
        yield break;
    }

}
