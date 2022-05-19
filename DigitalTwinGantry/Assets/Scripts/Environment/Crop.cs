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

	/// <summary>
	/// The crop will be removed when its current time period equals this flag.
	/// </summary>
	private const InteractableFlag INSTANTLY_REMOVE_CROP_FLAG = InteractableFlag.NONE;

	[SerializeField] private AgrobotInteractable m_interactable;
	[SerializeField] private GameObject m_postSowingModel;
	[SerializeField] private TimePeriod[] m_timePeriods;
	public TimePeriod[] TimePeriods { get { return m_timePeriods; } }

	private Action<Crop> m_onHarvestCallback;
	private TimePeriod m_currentTimePeriod;
	private int m_timePeriodOffset; //offset the current crop timeperiod from the real timeperiod received in UpdateTimePeriod(int newTimePeriod)

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

		if (m_currentTimePeriod.InteractableFlags == INSTANTLY_REMOVE_CROP_FLAG)
		{
			m_onHarvestCallback(this); //chunk will create new crops to be sown
		}
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

	/// <summary>
	/// Returns the difference in timeperiods from one timeperiod to another. This distance will be a value from -(TIME_PERIOD_COUNT / 2) 
	/// through (TIME_PERIOD_COUNT / 2) with regard for timeperiods looping between TIME_PERIOD_COUNT and 0.
	/// </summary>
	/// <returns>the time difference from fromTimePeriod to toTimePeriod</returns>
	public static int DistanceBetween(int fromTimePeriod, int toTimePeriod) //Distance
	{
		int difference = fromTimePeriod - toTimePeriod;

		if (difference > TIME_PERIOD_COUNT / 2)
		{
			difference = TIME_PERIOD_COUNT - difference;
		}
		else if (difference < -TIME_PERIOD_COUNT / 2)
		{
			difference = TIME_PERIOD_COUNT + difference;
			difference = -difference;
		}
		else
		{
			difference = -difference;
		}

		return difference;
	}

	/// <summary>
	/// Returns what the timeperiod would be if time changed by a certain amount of timeperiods.
	/// When the timeperiod goes over TIME_PERIOD_COUNT it loops to 0 and continues counting.
	/// If the timeperiod goes under 0 it loops to TIME_PERIOD_COUNT and continues counting.
	/// </summary>
	/// <param name="timePeriod">the timeperiod to start from</param>
	/// <param name="timeChange">by how many timeperiods time should change(can be negative)</param>
	/// <returns>what timePeriod would be if time advanced by timeChange timeperiods</returns>
	public static int CalculatTimePeriod(int timePeriod, int timeChange)//PeriodIfTimeChanged
	{
		timePeriod += timeChange;
		timePeriod = timePeriod % TIME_PERIOD_COUNT;

		if (timePeriod < 0)
		{
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
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "path")
		{
			Destroy(this.gameObject);
			m_onHarvestCallback(this);
		}
	}
}