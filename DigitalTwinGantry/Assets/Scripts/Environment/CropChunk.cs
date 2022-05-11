using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropChunk : MonoBehaviour
{
    [SerializeField] private float sizeBetweenCrop;

    private float xSize;
    private float ySize;

    private List<GameObject> m_crops;

    public void Init(GameObject cropType, Vector2 chunkSize)
    {
        m_crops = new List<GameObject>();
        xSize = chunkSize.x;
        ySize = chunkSize.y;

        GenerateChunk(cropType);
    }

    public void GenerateChunk(GameObject cropType)
    {
		// Generate the chunks
		for (float x = 0; x < xSize; x += sizeBetweenCrop)
		{
			for (float z = 0; z < ySize; z += sizeBetweenCrop)
			{
				GameObject crop = Instantiate(cropType, transform);
                crop.transform.localPosition = new Vector3(x, transform.position.y, z);
				m_crops.Add(crop);
			}
		}
    }

    public void UpdateTimePeriod(int timePeriod)
    {
        for (int i = 0; i < m_crops.Count; i++)
        {
            m_crops[i].GetComponent<Crop>().UpdateTimePeriod(timePeriod);
        }
    }
}
