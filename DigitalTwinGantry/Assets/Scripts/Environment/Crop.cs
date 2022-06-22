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
	/// The crop will be removed when its current time period equals this flag.
	/// </summary>
	private const InteractableFlag INSTANTLY_REMOVE_CROP_FLAG = InteractableFlag.NONE;

	[SerializeField] private AgrobotInteractable m_interactable;

	[SerializeField] private GameObject m_postSowingModel;
    public GameObject PostSowingModel => m_postSowingModel;

	[SerializeField] private TimePeriod[] m_timePeriods;
	public TimePeriod[] TimePeriods { get { return m_timePeriods; } }

	private Action<Crop, bool> m_onHarvestCallback;
	private TimePeriod m_currentTimePeriod;
	private int m_timePeriodOffset; //offset the current crop timeperiod from the real timeperiod received in UpdateTimePeriod(int newTimePeriod)

    public GameObject CurrentModel => m_currentTimePeriod.Model;

	/// <summary>
	/// Initializes the crop, call this function right after instantiating the crop.
	/// </summary>
	/// <param name="currentTimePeriod">The current time periode this crop is on</param>
	/// <param name="timePeriodOffset">The internal time period offset of this crop (is the crop for example sowed 2 time periods too early?)</param>
	/// <param name="onHarvestCallback">This function will be called when this crop gets harvested</param>
	public void Initialize(int currentTimePeriod, int timePeriodOffset, Action<Crop, bool> onHarvestCallback)
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
		m_currentTimePeriod = m_timePeriods[TimePeriod.PeriodIfTimeChanged(newTimePeriod, m_timePeriodOffset)];
		m_currentTimePeriod.Model.SetActive(true);

		m_interactable.SetFlags(m_currentTimePeriod.InteractableFlags);

		if (m_currentTimePeriod.InteractableFlags == INSTANTLY_REMOVE_CROP_FLAG)
		{
			m_onHarvestCallback(this, true); //chunk will create new crops to be sown
		}
	}

	public void OnInteract(AgrobotAction action)
	{
		if (action.Flags.HasFlag(InteractableFlag.SOW))
		{
			//change to the model of a recently sown crop
			m_currentTimePeriod.Model.SetActive(false);
			m_postSowingModel.SetActive(true);
		}

		if (action.Flags.HasFlag(InteractableFlag.HARVEST) || action.Flags.HasFlag(InteractableFlag.UPROOT))
		{
			m_currentTimePeriod.Model.SetActive(false);
			m_onHarvestCallback(this, true);
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// Returns the index of a timeperiod that has the specified interactable flag. 
	/// If there are multiple timeperiods with the flag this will return the one closest to the parameter.
	/// If there are no timeperiods with the flag this will return int.MaxValue.
	/// </summary>
	/// <param name="comparedTimePeriod">the timeperiod the returned value should be closest to</param>
	/// <returns>the index of a timeperiod that has the specified interactable flag</returns>
	public int GetNearestTimePeriod(int comparedTimePeriod, InteractableFlag flag)
	{
		//find all time periods with the specified flag
		List<int> timePeriods = new List<int>();
		for (int i = 0; i < TimePeriod.TIME_PERIOD_COUNT; i++)
		{
			if (m_timePeriods[i].InteractableFlags.HasFlag(flag))
			{
				timePeriods.Add(i);
			}
		}

		//find the one nearest to the parameter timeperiod
		int nearestIndex = int.MaxValue;
		foreach (int index in timePeriods)
		{
			if (Mathf.Abs(TimePeriod.Distance(comparedTimePeriod, index)) < Mathf.Abs(TimePeriod.Distance(comparedTimePeriod, nearestIndex)))
			{
				nearestIndex = index;
			}
		}
		return nearestIndex;
	}

	private void OnValidate()
	{
		if (m_timePeriods.Length != TimePeriod.TIME_PERIOD_COUNT)
		{
			Debug.LogWarning("Each crop must have exactly " + TimePeriod.TIME_PERIOD_COUNT + " time periods!");
			Array.Resize(ref m_timePeriods, TimePeriod.TIME_PERIOD_COUNT);
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "path")
		{
			Destroy(this.gameObject);
			m_onHarvestCallback(this, false);
		}
	}
}