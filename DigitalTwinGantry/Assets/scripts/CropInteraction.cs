using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropInteraction
{
   public void addInteraction(GameObject land)
    {
        Renderer landRenderer = land.GetComponent<Renderer>();
        float scale = 0.2f;
        for(float i = landRenderer.bounds.min.x; i <landRenderer.bounds.max.x; i+=1)
        {
            for (float j = landRenderer.bounds.min.z; j < landRenderer.bounds.max.z; j+=1)
            {
                GameObject interactionCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                interactionCube.transform.localScale = new Vector3(scale, scale, scale);
                interactionCube.transform.position = new Vector3(i + scale/2, 1.1f, j+scale/2);
                Renderer cubeRenderer = interactionCube.GetComponent<Renderer>();
                cubeRenderer.material.color = Color.gray;
                interactionCube.transform.parent = land.transform;
            }
        }
    }
}
