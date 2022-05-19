using UnityEngine;

[System.Serializable]
public partial struct TimePeriod
{
	/// <summary>
	/// The amount of time periods that every croptype has.
	/// </summary>
	public const int TIME_PERIOD_COUNT = 12;

	public GameObject Model;
	public InteractableFlag InteractableFlags;

	/// <summary>
	/// Returns the difference in timeperiods from one timeperiod to another. This distance will be a value from -(TIME_PERIOD_COUNT / 2) 
	/// through (TIME_PERIOD_COUNT / 2) with regard for timeperiods looping between TIME_PERIOD_COUNT and 0.
	/// </summary>
	/// <returns>the time difference from fromTimePeriod to toTimePeriod</returns>
	public static int Distance(int fromTimePeriod, int toTimePeriod)
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
	public static int PeriodIfTimeChanged(int timePeriod, int timeChange)
	{
		timePeriod += timeChange;
		timePeriod = timePeriod % TIME_PERIOD_COUNT;

		if (timePeriod < 0)
		{
			timePeriod += TIME_PERIOD_COUNT;
		}

		return timePeriod;
	}
}