using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A crop has a gameobject (appearance) and interactableflags defined for each time period.
/// </summary>
public class Crop : MonoBehaviour
{
	/// <summary>
	/// The amount of time periods that every croptype has.
	/// </summary>
	public const int TIME_PERIOD_COUNT = 12;

	[SerializeField] private AgrobotInteractable m_interactable;
	[SerializeField] private GameObject m_postSowingModel;
	[SerializeField] private TimePeriod[] m_timePeriods;
	public TimePeriod[] TimePeriods {get {return m_timePeriods;}}

	private Action<Crop> m_onHarvestCallback;
	private TimePeriod m_currentTimePeriod;
	private int m_timePeriodOffset; //offset the current crop timeperiod from the real timeperiod received in UpdateTimePeriod(int newTimePeriod)
	//chunks might decide to grow a crop a little earlier or later after the previous was harvested

	[System.Serializable]
	public struct TimePeriod
	{
		public GameObject Model;
		public InteractableFlag InteractableFlags;
	}

	public void Initialize(int currentTimePeriod, int timePeriodOffset, Action<Crop> onHarvestCallback)
	{
		m_onHarvestCallback = onHarvestCallback;
		m_timePeriodOffset = timePeriodOffset;
		m_currentTimePeriod = m_timePeriods[0];

		foreach (TimePeriod timePeriod in m_timePeriods)
		{
			timePeriod.Model.SetActive(false);
		}

		UpdateTimePeriod(currentTimePeriod);
	}

	public void UpdateTimePeriod(int newTimePeriod)
	{
		m_currentTimePeriod.Model.SetActive(false);
		m_currentTimePeriod = m_timePeriods[CalculatTimePeriod(newTimePeriod, m_timePeriodOffset)];
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
			m_onHarvestCallback(this);
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
			if (Mathf.Abs(DistanceBetween(comparedTimePeriod, index)) < Mathf.Abs(DistanceBetween(comparedTimePeriod, nearestIndex)))
			{
				nearestIndex = index;
			}
		}
		return nearestIndex;
	}

	/// <returns>the difference between two timeperiods</returns>
	public int DistanceBetween(int timePeriod1, int timePeriod2)
	{
		int difference = timePeriod1 - timePeriod2;

		if (difference > TIME_PERIOD_COUNT / 2)
		{
			difference = TIME_PERIOD_COUNT - difference;
		} else if (difference < -TIME_PERIOD_COUNT / 2) 
		{
			difference = TIME_PERIOD_COUNT + difference;
			difference = -difference;
		} else
		{
			difference = -difference;
		}

		return difference;
	}

	public int CalculatTimePeriod(int timePeriod, int difference) {
		timePeriod += difference;
		timePeriod = timePeriod % TIME_PERIOD_COUNT;

		if (timePeriod < 0) {
			timePeriod += TIME_PERIOD_COUNT;
		}

		return timePeriod;
	}

	private void OnValidate()
	{
		if (m_timePeriods.Length != TIME_PERIOD_COUNT)
		{
			Debug.LogWarning("Each crop must have exactly " + TIME_PERIOD_COUNT + " time periods!");
			Array.Resize(ref m_timePeriods, TIME_PERIOD_COUNT);
		}
	}
}