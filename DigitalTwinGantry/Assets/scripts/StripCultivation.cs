using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StripCultivation : MonoBehaviour
{
    private float farmWidth;
    private float pathWidth;
    [SerializeField] private GameObject land;

    public void setWidth(float farmWidth, float pathWidth)
    {
        this.farmWidth = farmWidth;
        this.pathWidth = pathWidth;
    }
    void Start()
    {
        Transform grandParent = gameObject.transform.root;
        float scaleFactorPos = grandParent.localScale.x;
        int scaleFactor = 10; //The plane is exactly 10 times lager then the cube. change if needed with model scale factor
        Transform ground = gameObject.transform.parent;
        float offset = (farmWidth + pathWidth) / 2;
        for (float i = scaleFactorPos*-10/2; i <= scaleFactorPos*10/2; i += (farmWidth + pathWidth)*scaleFactorPos)
        {
            GameObject cube = Instantiate(land);
            cube.transform.localScale = new Vector3(farmWidth - pathWidth, 1, grandParent.localScale.z*scaleFactor);
            cube.transform.position = new Vector3(ground.position.x + i + offset, ground.position.y, ground.position.z);
            cube.GetComponent<Renderer>().material.color = Color.green;
            cube.transform.parent = this.gameObject.transform;
        }
    }
}
