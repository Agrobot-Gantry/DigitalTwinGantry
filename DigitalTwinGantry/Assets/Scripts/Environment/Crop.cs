using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
	[SerializeField]
	private TimePeriod[] m_timePeriods;

	private delegate void OnHarvestCallback(Crop crop);
	private OnHarvestCallback m_callback;
	private int m_timePeriodOffset;

	[System.Serializable]
	private struct TimePeriod
	{
		public GameObject Model;
		public InteractableFlag InteractableFlags;
	}

	public void Initialize(CropField cropField, int timePeriodOffset)
	{
		m_callback = new OnHarvestCallback(cropField.OnCropHarvested);
		m_timePeriodOffset = timePeriodOffset;
	}

	public void UpdateTimePeriod(int timePeriod)
	{
		//TODO
		//keep offset in mind then updating
	}

	public void OnInteract(AgrobotAction action)
	{
		//TODO
		if (action.GetFlags().HasFlag(InteractableFlag.HARVEST))
		{
			m_callback(this);
		}
	}

	public int GetSowingTimePeriod()
	{
		//TODO
		return 0;
	}
}
