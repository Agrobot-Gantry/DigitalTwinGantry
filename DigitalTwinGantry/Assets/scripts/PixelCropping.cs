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
        Transform ground = gameObject.transform.parent;
        float offset = (farmWidth + pathWidth) / 2;
        int k = 0;
        for (float i = scaleFactorPosX * -10 / 2; i < scaleFactorPosX * 10 / 2; i += (farmWidth + pathWidth) * scaleFactorPosX)
        {
            for (float j = scaleFactorPosZ * -10 / 2; j < scaleFactorPosZ * 10 / 2; j += (farmWidth + pathWidth) * scaleFactorPosZ)
            {
                CropInteraction cropInteraction = new CropInteraction();
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localScale = new Vector3(farmWidth - pathWidth, 1, farmWidth - pathWidth);
                cube.transform.position = new Vector3(i + offset, ground.position.y, ground.position.z + j + offset);
                if (k==0) cube.GetComponent<Renderer>().material.color = Color.black;
                else cube.GetComponent<Renderer>().material.color = Color.blue;
                k++;
                cube.transform.parent = this.gameObject.transform;
                cropInteraction.addInteraction(cube);
                
            }
        }
    }

}
