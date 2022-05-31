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
        InKinematicArm arm = m_tool.GetToolObject().GetComponent<InKinematicArm>();
		arm.ResetReach();

        yield return arm.ReachForPointSmooth(m_targetInteractable.transform.position, 0.1f, AgrobotDefinitions.Instance.EquipmentSpeed);

        // while (m_targetInteractable != null && Vector3.Distance(m_tool.GetToolObject().transform.position, m_targetInteractable.transform.position) > 0.1f)
		// {
		// 	m_tool.GetToolObject().transform.position = Vector3.MoveTowards(
		// 		m_tool.GetToolObject().transform.position,
		// 		m_targetInteractable.transform.position,
		// 		AgrobotDefinitions.Instance.EquipmentSpeed * TimeChanger.DeltaTime);
		// 	yield return null;
		// }

		if (m_targetInteractable != null)
		{
			m_targetInteractable.OnInteract(this);
		}
		else Debug.Log("TargetInteractable is null");
		Finish();
	}
}
