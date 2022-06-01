using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple example of an action.
/// </summary>
public class HarvestAction : AgrobotAction
{
	private AgrobotTool m_tool;

	public HarvestAction(AgrobotInteractable target, AgrobotBehaviour behaviour, AgrobotEquipment equipment) : base(target, behaviour, equipment)
	{
		m_tool = equipment.GetTool(GetFlags());
	}

	public override InteractableFlag GetFlags()
	{
		return InteractableFlag.HARVEST;
	}

	public override IEnumerator Start()
    {
        Vector3 target = new Vector3(m_targetInteractable.transform.position.x,
            m_tool.GetToolObject().transform.position.y, m_tool.GetToolObject().transform.position.z);
  
		while (m_targetInteractable != null && Vector3.Distance(m_tool.GetToolObject().transform.position, target) > 0.1f)
        {
        	m_tool.GetToolObject().transform.position = Vector3.MoveTowards(
        		m_tool.GetToolObject().transform.position, target,
        		AgrobotDefinitions.Instance.EquipmentSpeed * TimeChanger.DeltaTime);
        	yield return null;
        }

		InKinematicArm arm = m_tool.GetToolObject().GetComponent<InKinematicArm>();
        yield return arm.ReachForPointSmooth(m_targetInteractable.transform.position, 0.1f, AgrobotDefinitions.Instance.EquipmentSpeed);

        if (m_targetInteractable != null)
		{
			m_targetInteractable.OnInteract(this);
		}
		else Debug.Log("TargetInteractable is null");
		Finish();
	}
}
