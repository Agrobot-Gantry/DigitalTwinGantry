using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StripCultivation : MonoBehaviour
{
    private float farmWidth;
    private float pathWidth;

    public void setWidth(float farmWidth, float pathWidth)
    {
        this.farmWidth = farmWidth;
        this.pathWidth = pathWidth;
    }
    void Start()
    {
        Transform grandParent = gameObject.transform.root;
        float scaleFactorPos = grandParent.localScale.x;
        int scaleFactor = 2; //from plane is exactly 10 times lager then cube. change if needed with model scale factor
        Transform ground = gameObject.transform.parent;
        for (float i = scaleFactorPos*-10/2; i <= scaleFactorPos*10/2; i += (farmWidth + pathWidth)*scaleFactorPos)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.parent = this.gameObject.transform;
            cube.transform.localScale = new Vector3(farmWidth - pathWidth, 1, ground.localScale.z*scaleFactor);
            cube.transform.position = new Vector3(i, ground.position.y, ground.position.z);
            cube.GetComponent<Renderer>().material.color = Color.green;
        }
    }
}
