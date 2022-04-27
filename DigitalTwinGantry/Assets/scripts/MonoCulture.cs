using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoCulture : MonoBehaviour
{
    private void Start()
    {
        CropInteraction cropInteraction = new CropInteraction();
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        
        Transform ground = gameObject.transform.parent;
        cube.transform.position = new Vector3(ground.position.x, ground.position.y, ground.position.z);
        int scaleFactor = 10; //The plane is exactly 10 times lager then the cube. change if needed with model scale factor
        cube.transform.localScale = new Vector3(ground.localScale.x*scaleFactor, 1, ground.localScale.z*scaleFactor);
        cube.GetComponent<Renderer>().material.color = Color.red;
        cube.transform.parent = this.gameObject.transform;
        cropInteraction.addInteraction(cube);
    }
}
