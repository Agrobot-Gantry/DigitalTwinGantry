using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
	/// <summary>
	/// The amount of time periods that every croptype has.
	/// </summary>
	public const int TIME_PERIOD_COUNT = 12;

	[SerializeField] private AgrobotInteractable m_interactable;
	[SerializeField] private GameObject m_postSowingModel;
	[SerializeField] private TimePeriod[] m_timePeriods;

	private delegate void OnHarvestCallback(Crop crop);
	private OnHarvestCallback m_callback;
	private TimePeriod m_currentTimePeriod;
	private int m_timePeriodOffset; //chunks might decide to grow a crop a little earlier or later after the previous was harvested

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
		m_currentTimePeriod = m_timePeriods[0];

		foreach (TimePeriod timePeriod in m_timePeriods)
		{
			timePeriod.Model.SetActive(false);
		}
	}

	public void UpdateTimePeriod(int newTimePeriod)
	{
		m_currentTimePeriod.Model.SetActive(false);
		m_currentTimePeriod = m_timePeriods[(newTimePeriod + m_timePeriodOffset) % TIME_PERIOD_COUNT];
		m_currentTimePeriod.Model.SetActive(true);

		m_interactable.SetFlags(m_currentTimePeriod.InteractableFlags);
	}

	public void OnInteract(AgrobotAction action)
	{
		if (action.GetFlags().HasFlag(InteractableFlag.SOW))
		{
			//change to the model of a recently sown crop
			m_currentTimePeriod.Model.SetActive(false);
			m_postSowingModel.SetActive(true);
		}

		if (action.GetFlags().HasFlag(InteractableFlag.HARVEST))
		{
			m_currentTimePeriod.Model.SetActive(false);
			m_callback(this);
		}
	}

	/// <summary>
	/// Returns the index of a timeperiod that has the SOW interactable flag. 
	/// If there are multiple timeperiods with the SOW flag this will return the one closest to the parameter.
	/// If there are no timeperiods with the SOW flag this will return int.MaxValue.
	/// </summary>
	/// <param name="comparedTimePeriod">the timeperiod the returned value should be closest to</param>
	/// <returns>the index of a timeperiod that has the SOW interactable flag</returns>
	public int GetNearestSowingTimePeriod(int comparedTimePeriod)
	{
		//find all time periods with the sowing flag
		List<int> sowingTimePeriods = new List<int>();
		for (int i = 0; i < TIME_PERIOD_COUNT; i++)
		{
			if (m_timePeriods[i].InteractableFlags.HasFlag(InteractableFlag.SOW))
			{
				sowingTimePeriods.Add(i);
			}
		}

		//find the one nearest to the parameter timeperiod
		int nearestIndex = int.MaxValue;
		foreach (int index in sowingTimePeriods)
		{
			if (DistanceBetween(comparedTimePeriod, index) < DistanceBetween(comparedTimePeriod, nearestIndex))
			{
				nearestIndex = index;
			}
		}
		return nearestIndex;
	}

	/// <returns>the difference between two timeperiods</returns>
	private int DistanceBetween(int timePeriod1, int timePeriod2)
	{
		int difference = Math.Abs(timePeriod1 - timePeriod2);
		if (difference > TIME_PERIOD_COUNT / 2)
		{
			difference = TIME_PERIOD_COUNT - difference;
		}
		return difference;
	}

	void OnValidate()
	{
		if (m_timePeriods.Length != TIME_PERIOD_COUNT)
		{
			Debug.LogError("Each crop must have exactly " + TIME_PERIOD_COUNT + " time periods!");
			System.Array.Resize(ref m_timePeriods, TIME_PERIOD_COUNT);
		}
	}
}