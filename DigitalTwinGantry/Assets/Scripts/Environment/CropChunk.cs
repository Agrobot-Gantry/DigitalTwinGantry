using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropChunk : MonoBehaviour
{
    [SerializeField] private float m_sizeBetweenCrop;
    [SerializeField] private float m_cropScale;

    private float m_xSize;
    private float m_ySize;

    private List<GameObject> m_crops;
    private Action<CropChunk, bool> m_onCHunkEmpty;

    private int m_timePeriod;
    private bool m_regenerateChunk = false;

    public void Initialize(GameObject cropType, Vector2 chunkSize, int timePeriod, Action<CropChunk, bool> onChunkEmpty)
    {
        m_crops = new List<GameObject>();
        m_xSize = chunkSize.x;
        m_ySize = chunkSize.y;

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
		for (float x = 0; x < m_xSize; x += m_sizeBetweenCrop)
		{
			for (float z = 0; z < m_ySize; z += m_sizeBetweenCrop)
			{
				GameObject cropObject = Instantiate(cropType, transform);
                cropObject.transform.localPosition = new Vector3(x, transform.position.y, z);
                cropObject.transform.localScale = new Vector3(m_cropScale, m_cropScale, m_cropScale);
				m_crops.Add(cropObject);

                Crop crop = cropObject.GetComponent<Crop>();
                crop.Initialize(m_timePeriod, offset, OnCropRemoved);
			}
		}
    }

    public void UpdateTimePeriod(int timePeriod)
    {
        m_timePeriod = timePeriod;

        List<GameObject> copiedCrops = new List<GameObject>(m_crops); //iterate through a copy because entries in the original might be deleted
        for (int i = 0; i < copiedCrops.Count; i++)
        {
            copiedCrops[i].GetComponent<Crop>().UpdateTimePeriod(timePeriod);
        }
    }

    /// <summary>
    /// Removes the crop from the chunk and calls back to the cropfield if all crops have been removed.
    /// If there are no crops to regenerate the chunk will not regenerate either.
    /// </summary>
    /// <param name="crop">the crop that was removed</param>
    /// <param name="regenerateCrop">wether the removed crop should be regenerated when the chunk gets regenerated</param>
    public void OnCropRemoved(Crop crop, bool regenerateCrop)
    {
        m_crops.Remove(crop.gameObject);

        if (regenerateCrop) //if one or more crops need to be regenered the chunk needs to be regenerated too
		{
            m_regenerateChunk = true;
		}

        if (m_crops.Count <= 0)
        {
            m_onCHunkEmpty(this, m_regenerateChunk);
        }
    }
}
