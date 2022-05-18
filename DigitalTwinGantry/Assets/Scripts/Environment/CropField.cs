using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class CropField : MonoBehaviour
{
	private const int MAX_SOWING_DISTANCE = 3;

	[Header("Sizes")]
	[SerializeField] private BoxCollider m_field;
	[SerializeField] private int m_xChunks;
	[SerializeField] private int m_yChunks;
	
	[Header("Agrobot")]
	[SerializeField] private AgrobotGantry m_agrobot;
	[SerializeField] private GameObject m_path;
	[SerializeField] private GameObject m_endZone;

	[Header("Crops")]
	[SerializeField] private GameObject m_ground;
	[SerializeField] private GameObject m_chunk;
	[SerializeField, Range(0, Crop.TIME_PERIOD_COUNT - 1)] private int m_startingMonth;
	[SerializeField] private GameObject[] m_cropTypes;

	[Header("Field change")]
	[SerializeField] private UnityEvent m_onFieldChange;
	
	private GameObject m_groundMesh;

	private Transform m_agrobotStart;
	private float m_gantryWidth = 3;
	private float m_gantryWheelWidth = 0.5f;

	private int m_currentMonth;
	public int CurrentMonth { get => m_currentMonth; }

	private List<GameObject> m_chunks;
	private List<GameObject> m_paths;

	private void Start()
	{
		if (m_agrobot != null) 
		{
			m_agrobotStart = new GameObject("Agrobot Start Pos").transform;
			m_agrobotStart.position = m_agrobot.transform.position;
			m_agrobotStart.rotation = m_agrobot.transform.rotation;
			m_gantryWidth = m_agrobot.GetGantryWidth();
			m_gantryWheelWidth = m_agrobot.GetGantryWheelWidth();
		}

		m_currentMonth = m_startingMonth;

		m_chunks = new List<GameObject>();
		m_paths = new List<GameObject>();
		GenerateChunks();
	}

	public void UpdateTimePeriod(int newTimePeriod)
	{
		m_onFieldChange.Invoke();

		// Set new time period
		newTimePeriod = Mathf.Clamp(newTimePeriod, 0, Crop.TIME_PERIOD_COUNT);
		m_currentMonth = newTimePeriod;

		// Update all chunks
		for (int i = 0; i < m_chunks.Count; i++)
		{
			m_chunks[i].GetComponent<CropChunk>().UpdateTimePeriod(m_currentMonth);
		}

		// Reset agrobot transform
		if (m_agrobot != null) 
		{
			m_agrobot.transform.position = m_agrobotStart.position;
			m_agrobot.transform.rotation = m_agrobotStart.rotation;
			m_agrobot.Reset();
		}
	}

	public void NextMonth()
	{
		m_currentMonth = Crop.CalculatTimePeriod(m_currentMonth, 1);
		UpdateTimePeriod(m_currentMonth);
	}

	public void OnChunkEmpty(CropChunk chunk)
	{
		Crop crop = GetSowableCrop();
		int offset = Crop.DistanceBetween(m_currentMonth, crop.GetNearestSowingTimePeriod(m_currentMonth));
		
		chunk.GenerateChunk(crop.gameObject, offset);
	}

	private void GenerateChunks()
	{
		// Remove all previous chunks and paths
		for (int i = 0; i < m_chunks.Count; i++)
		{
			Destroy(m_chunks[i]);
		}

		for (int i = 0; i < m_paths.Count; i++)
		{
			Destroy(m_paths[i]);
		}

		m_chunks.Clear();
		m_paths.Clear();

		float fieldWidth = m_field.size.x;
		float fieldHeight = m_field.size.z;

		float chunkWidth = fieldWidth / m_xChunks;
		float chunkHeight = fieldHeight / m_yChunks;

		// Generate the chunks
		for (int x = 0; x < m_xChunks; x++)
		{
			for (int z = 0; z < m_yChunks; z++)
			{
				GameObject chunkObject = Instantiate(m_chunk, new Vector3(m_field.bounds.min.x, transform.position.y, m_field.bounds.min.z) + 
					new Vector3(x * chunkWidth, 0, z * chunkHeight), Quaternion.Euler(0, 0, 0));

				CropChunk chunk = chunkObject.GetComponent<CropChunk>();
				chunk.Initialize(GetStartingCrop().gameObject, new Vector2(chunkWidth, chunkHeight), m_currentMonth, OnChunkEmpty);

				m_chunks.Add(chunkObject);
			}
		}


		// Generate the driving paths
		for (float x = m_field.bounds.min.x; x < m_field.bounds.max.x; x += m_gantryWidth)
		{
			GameObject path = Instantiate(m_path, new Vector3(x, transform.position.y, m_field.bounds.center.z), Quaternion.Euler(0, 0, 0));
			m_paths.Add(path);

			path.transform.localScale = new Vector3(m_gantryWheelWidth, 0.1f, fieldHeight + m_gantryWidth);
		}

		// Generate end zone
		GameObject endZone = Instantiate(m_endZone, new Vector3(m_field.bounds.max.x, transform.position.y, m_field.bounds.max.z), Quaternion.Euler(0, 0, 0));
		endZone.transform.localScale = new Vector3(m_gantryWidth, 0.1f, 1);
		// Get endzone script and add unityevent to script
		EndZone endZoneScript = endZone.GetComponent<EndZone>();
		endZoneScript.setEvent(NextMonth);


		// Remove and create the ground
		if (m_groundMesh != null)
		{
			Destroy(m_groundMesh);
		}

		m_groundMesh = Instantiate(m_ground, new Vector3(transform.position.x + m_field.center.x, transform.position.y, transform.position.z + m_field.center.z), Quaternion.Euler(0, 0, 0));
		m_groundMesh.transform.localScale = new Vector3(m_field.size.x, 0.01f, m_field.size.z);

		// Reset agrobot transform
		if (m_agrobot != null) 
		{
			m_agrobotStart.position = new Vector3(m_field.bounds.min.x + (m_gantryWidth / 2), m_field.bounds.max.y, m_field.bounds.min.z - (m_gantryWidth/2));
			m_agrobot.transform.position = m_agrobotStart.position;
			m_agrobot.transform.rotation = m_agrobotStart.rotation;
			m_agrobot.Reset();
		}
	}

	public void SetChunksX(int chunks)
	{
		m_xChunks = chunks;
		OnValidate();

		GenerateChunks();
	}

	public void SetChunksY(int chunks)
	{
		m_yChunks = chunks;
		OnValidate();

		GenerateChunks();
	}

	private Crop GetStartingCrop()
	{
		List<Crop> possibleCrops = new List<Crop>();
		foreach (GameObject cropObject in m_cropTypes)
		{
			Crop crop = cropObject.GetComponent<Crop>();
			if (AgrobotInteractable.FlagCount(crop.TimePeriods[m_currentMonth].InteractableFlags) > 0) 
			{
				possibleCrops.Add(crop);
			}
		}

		if (possibleCrops.Count > 0) 
		{
			return possibleCrops[UnityEngine.Random.Range(0, possibleCrops.Count)];
		}
		return m_cropTypes[UnityEngine.Random.Range(0, m_cropTypes.Length)].GetComponent<Crop>();
	}

	private Crop GetSowableCrop()
	{
		//sort the crop types based on how far away their normal sowing period is from the current timeperiod
		Array.Sort(m_cropTypes, (type1, type2) => 
		{
			Crop crop1 = type1.GetComponent<Crop>();
			Crop crop2 = type2.GetComponent<Crop>();
			
			return Mathf.Abs(Crop.DistanceBetween(m_currentMonth, crop1.GetNearestSowingTimePeriod(m_currentMonth))) - 
				Mathf.Abs(Crop.DistanceBetween(m_currentMonth, crop2.GetNearestSowingTimePeriod(m_currentMonth)));
		});

		//choose a random crop type that doesn't exceed the max sowing distance
		int index = UnityEngine.Random.Range(0, m_cropTypes.Length); //pick a random crop type
		while (index > 0) 
		{ //as long as we have other options left
			Crop crop = m_cropTypes[index].GetComponent<Crop>();
			if (Mathf.Abs(Crop.DistanceBetween(m_currentMonth, crop.GetNearestSowingTimePeriod(m_currentMonth))) <= MAX_SOWING_DISTANCE)
			{
				return crop; //return this crop if it's within the max sowing distance
			}
			index--; //if not keep looking
		}

		//index 0 is the last option left, use this crop even if it's further than the max sowing distance
		return m_cropTypes[index].GetComponent<Crop>();
	}

	private void OnValidate() 
	{
		m_xChunks = Mathf.Max(m_xChunks, 0);
		m_yChunks = Mathf.Max(m_yChunks, 0);
	}
}
