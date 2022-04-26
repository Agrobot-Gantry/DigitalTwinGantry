using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmLand : MonoBehaviour
{
    public enum FarmingType
    {
        Mono,
        Strips,
        Pixel,
    }
    [SerializeField]private FarmingType farmingType;
    private FarmingType prevFarmingType;
    private GameObject[] farmTypes = new GameObject[3];
    private GameObject activeLand;
    [SerializeField]private float landWidth = 4.0f;
    [SerializeField]private float pathWidth = 0.2f;

    public void setWidth(float widthGantry, float widthWheels)
    {
        this.landWidth = widthGantry;
        this.pathWidth = widthWheels;
    }
    // Start is called before the first frame update
    void Awake()
    {
        for(int i = 0; i < gameObject.transform.childCount; i++)
        {
            farmTypes[i] = gameObject.transform.GetChild(i).gameObject;
        }
        StripCultivation sc = farmTypes[1].GetComponent<StripCultivation>();
        PixelCropping pc = farmTypes[2].GetComponent<PixelCropping>();
        sc.setWidth(landWidth, pathWidth);
        pc.setWidth(landWidth, pathWidth);
        setCells();
    }

    // Update is called once per frame
    void Update()
    {
        if(farmingType != prevFarmingType)
        {
            prevFarmingType = farmingType;
            setCells();
        }
    }

    //give slider to this function for the switch
    private void setCells()
    {
       
        switch (farmingType){
            case FarmingType.Mono:
                activeLand = gameObject.transform.GetChild(0).gameObject;
                break;
            case FarmingType.Strips:
                activeLand = gameObject.transform.GetChild(1).gameObject;
                break;
            case FarmingType.Pixel:
                activeLand = gameObject.transform.GetChild(2).gameObject;
                break;
        }
        foreach (GameObject land in farmTypes)
        {
            if (land == activeLand) land.SetActive(true);
            else land.SetActive(false);
        }

    }


}
