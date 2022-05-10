using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropField : MonoBehaviour
{
	[SerializeField] private GameObject[] m_cropTypes;

	private List<Crop> m_crops;

	void Start()
	{
		//Crop cropscript = crop.GetComponent<Crop>();
	}

	public void UpdateTimePeriod(int newTimePeriod)
	{
		foreach (Crop crop in m_crops)
		{
			crop.UpdateTimePeriod(newTimePeriod);
		}
	}

	public void OnCropHarvested(Crop crop)
	{
		//TODO
	}
}
