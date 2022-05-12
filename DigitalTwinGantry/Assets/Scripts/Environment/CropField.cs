using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropField : MonoBehaviour
{
	[Header("Sizes")]
	[SerializeField] private BoxCollider m_field;
	[SerializeField] private int m_xChunks;
	[SerializeField] private int m_yChunks;
	
	[Header("Agrobot")]
	[SerializeField] private AgrobotGantry m_agrobot;
	[SerializeField] private GameObject m_path;

	[Header("Crops")]
	[SerializeField] private GameObject m_chunk;
	[SerializeField, Range(0, Crop.TIME_PERIOD_COUNT)] private int m_startingMonth;
	[SerializeField] private GameObject[] m_cropTypes;

	private Transform m_agrobotStart;
	private float m_gantryWidth;
	private float m_gantryWheelWidth;

	private int m_currentMonth;

	private List<GameObject> m_chunks;
	private List<GameObject> m_paths;

	private void Start()
	{
		m_agrobotStart = new GameObject("Agrobot Start Pos").transform;
		m_agrobotStart.position = m_agrobot.transform.position;
		m_agrobotStart.rotation = m_agrobot.transform.rotation;
		m_gantryWidth = m_agrobot.GetGantryWidth();
		m_gantryWheelWidth = m_agrobot.GetGantryWheelWidth();

		m_currentMonth = m_startingMonth;

		m_chunks = new List<GameObject>();
		m_paths = new List<GameObject>();
		GenerateChunks();
	}

	public void UpdateTimePeriod(int newTimePeriod)
	{
		// Set new time period
		newTimePeriod = Mathf.Clamp(newTimePeriod, 0, Crop.TIME_PERIOD_COUNT);
		m_currentMonth = newTimePeriod;

		// Update all chunks
		for (int i = 0; i < m_chunks.Count; i++)
		{
			m_chunks[i].GetComponent<CropChunk>().UpdateTimePeriod(m_currentMonth);
		}

		// Reset agrobot transform
		m_agrobot.transform.position = m_agrobotStart.position;
		m_agrobot.transform.rotation = m_agrobotStart.rotation;
	}

	public void NextMonth()
	{
		m_currentMonth++;
		UpdateTimePeriod(m_currentMonth);
	}

	public void OnChunkEmpty(CropChunk chunk)
	{
		GameObject crop = m_cropTypes[Random.Range(0, m_cropTypes.Length)];
		chunk.GenerateChunk(crop);
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
				chunk.Init(m_cropTypes[Random.Range(0, m_cropTypes.Length)], new Vector2(chunkWidth, chunkHeight));

				m_chunks.Add(chunkObject);
			}
		}

		// Generate the driving paths
		for (float x = m_field.bounds.min.x; x < m_field.bounds.max.x; x += m_gantryWidth)
		{
			GameObject path = Instantiate(m_path, new Vector3(x, transform.position.y, m_field.bounds.center.z), Quaternion.Euler(0, 0, 0));
			m_paths.Add(path);

			path.transform.localScale = new Vector3(m_gantryWheelWidth, 0.1f, fieldHeight);
		}

		m_agrobotStart.position = new Vector3(m_field.bounds.min.x + (m_gantryWidth / 2), m_field.bounds.max.y, m_field.bounds.min.z);

		// Reset agrobot transform
		m_agrobot.transform.position = m_agrobotStart.position;
		m_agrobot.transform.rotation = m_agrobotStart.rotation;
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

	private void OnValidate() 
	{
		m_xChunks = Mathf.Max(m_xChunks, 0);
		m_yChunks = Mathf.Max(m_yChunks, 0);
	}
}
