using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelCropping : MonoBehaviour
{
    private float farmWidth;
    private float pathWidth;
    public void setWidth(float farmWidth, float pathWidth)
    {
        this.farmWidth = farmWidth;
        this.pathWidth = pathWidth;
    }
    // Start is called before the first frame update
    void Start()
    {
        Transform grandParent = gameObject.transform.root;
        float scaleFactorPosX = grandParent.localScale.x;
        float scaleFactorPosZ = grandParent.localScale.z;
        int scaleFactor = 2; //from plane is exactly 10 times lager then cube. change if needed with model scale factor
        Transform ground = gameObject.transform.parent;
        for (float i = scaleFactorPosX * -10 / 2; i < scaleFactorPosX * 10 / 2; i += (farmWidth + pathWidth) * scaleFactorPosX)
        {
            for (float j = scaleFactorPosZ * -10 / 2; j < scaleFactorPosZ * 10 / 2; j += (farmWidth + pathWidth) * scaleFactorPosZ)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.parent = this.gameObject.transform;
                cube.transform.localScale = new Vector3(farmWidth - pathWidth, 1, farmWidth - pathWidth);
                cube.transform.position = new Vector3(i, ground.position.y, ground.position.z + j);
                cube.GetComponent<Renderer>().material.color = Color.blue;
            }
        }
    }

}
