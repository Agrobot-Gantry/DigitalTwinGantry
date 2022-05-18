using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropChunk : MonoBehaviour
{
    [SerializeField] private float sizeBetweenCrop;

    private float xSize;
    private float ySize;

    private List<GameObject> m_crops;
    private Action<CropChunk> m_onCHunkEmpty;

    private int m_timePeriod;

    public void Initialize(GameObject cropType, Vector2 chunkSize, int timePeriod, Action<CropChunk> onChunkEmpty)
    {
        m_crops = new List<GameObject>();
        xSize = chunkSize.x;
        ySize = chunkSize.y;

        m_timePeriod = timePeriod;
        m_onCHunkEmpty = onChunkEmpty;

        GenerateChunk(cropType, 0);
    }

    public void GenerateChunk(GameObject cropType, int offset)
    {
        for (int i = 0; i < m_crops.Count; i++)
        {
            Destroy(m_crops[i]);
        }

        m_crops.Clear();

		// Generate the crops
		for (float x = 0; x < xSize; x += sizeBetweenCrop)
		{
			for (float z = 0; z < ySize; z += sizeBetweenCrop)
			{
				GameObject cropObject = Instantiate(cropType, transform);
                cropObject.transform.localPosition = new Vector3(x, transform.position.y, z);
				m_crops.Add(cropObject);

                Crop crop = cropObject.GetComponent<Crop>();
                crop.Initialize(m_timePeriod, offset, OnCropRemoved);
			}
		}
    }

    public void UpdateTimePeriod(int timePeriod)
    {
        m_timePeriod = timePeriod;

        for (int i = 0; i < m_crops.Count; i++)
        {
            m_crops[i].GetComponent<Crop>().UpdateTimePeriod(timePeriod);
        }
    }

    public void OnCropRemoved(Crop crop)
    {
        m_crops.Remove(crop.gameObject);

        if (m_crops.Count <= 0)
        {
            m_onCHunkEmpty(this);
        }
    }
}
