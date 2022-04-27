using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropInteraction : MonoBehaviour
{
    [SerializeField] private GameObject crop;
   public void Start()
    {
        StartCoroutine(setCrops());
    }

    private IEnumerator setCrops()
    {
        yield return null;
        Collider landRenderer = gameObject.GetComponent<Collider>();
        for (float i = landRenderer.bounds.min.x; i < landRenderer.bounds.max.x; i += 1)
        {
            for (float j = landRenderer.bounds.min.z; j < landRenderer.bounds.max.z; j += 1)
            {
                GameObject interactionCube = Instantiate(crop);
                interactionCube.transform.position = new Vector3(i + crop.transform.localScale.x / 2, 1, j + crop.transform.localScale.z / 2);
                interactionCube.transform.parent = gameObject.transform;
            }
        }
    }
}
