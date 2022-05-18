using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IrrigationAction : AgrobotAction
{
	private AgrobotTool m_tool;

	public IrrigationAction(AgrobotInteractable target, AgrobotBehaviour behaviour, AgrobotEquipment equipment) : base(target, behaviour, equipment)
	{
		m_tool = equipment.GetTool(GetFlags());
	}

	public override InteractableFlag GetFlags()
	{
		return InteractableFlag.WATER;
	}

	public override IEnumerator Start()
	{
		while (Vector3.Distance(m_tool.GetToolObject().transform.position, m_targetInteractable.transform.position) > 0.1f)
		{
			m_tool.GetToolObject().transform.position = Vector3.MoveTowards(
				m_tool.GetToolObject().transform.position,
				m_targetInteractable.transform.position,
				TimeChanger.DeltaTime);
			yield return null;
		}
		m_targetInteractable.OnInteract(this);
		Finish();
		yield break;
	}
}
