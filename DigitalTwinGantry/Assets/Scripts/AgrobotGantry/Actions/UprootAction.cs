using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UprootAction : AgrobotAction
{
	private AgrobotTool m_tool;

	public UprootAction(AgrobotInteractable target, AgrobotBehaviour behaviour, AgrobotEquipment equipment) : base(target, behaviour, equipment)
	{
		m_tool = equipment.GetTool(GetFlags());
	}

	public override InteractableFlag GetFlags()
	{
		return InteractableFlag.UPROOT;
	}

	public override IEnumerator Start()
	{
        m_tool.busy = true;

        Vector3 target = new Vector3(m_targetInteractable.transform.position.x,
            m_tool.GetToolObject().transform.position.y, m_tool.GetToolObject().transform.position.z);

        while (m_targetInteractable != null && Vector3.Distance(m_tool.GetToolObject().transform.position, target) > 0.1f)
        {
            target = new Vector3(m_targetInteractable.transform.position.x,
                m_tool.GetToolObject().transform.position.y, m_tool.GetToolObject().transform.position.z);

            m_tool.GetToolObject().transform.position = Vector3.MoveTowards(
                m_tool.GetToolObject().transform.position, target,
                AgrobotDefinitions.Instance.EquipmentSpeed * TimeChanger.DeltaTime);

            yield return null;
        }

        AgrobotArm arm = m_tool.GetToolObject().GetComponent<AgrobotArm>();
        yield return arm.ReachForPointSmooth(m_targetInteractable.transform, 0.1f, AgrobotDefinitions.Instance.EquipmentSpeed);
        yield return new WaitForSeconds(0.2f);
		arm.ReturnToBase(AgrobotDefinitions.Instance.EquipmentSpeed * 1.2f);

        if (m_targetInteractable != null)
        {
            m_targetInteractable.OnInteract(this);
        }
        else Debug.Log("TargetInteractable is null");
        m_tool.busy = false;
        Finish();

		// while (m_targetInteractable != null && Vector3.Distance(m_tool.GetToolObject().transform.position, m_targetInteractable.transform.position) > 0.1f)
		// 	{
		// 	m_tool.busy = true;
		// 	m_tool.GetToolObject().transform.position = Vector3.MoveTowards(
		// 			m_tool.GetToolObject().transform.position,
		// 			m_targetInteractable.transform.position,
		// 			AgrobotDefinitions.Instance.EquipmentSpeed * TimeChanger.DeltaTime);
		// 		yield return null;
		// 	}
		// if (m_targetInteractable != null)
		// {
		// 	m_targetInteractable.OnInteract(this);
		// }
		// else Debug.Log("TargetInteractable is null");
		// m_tool.busy = false;
		// Finish();
	}
}
