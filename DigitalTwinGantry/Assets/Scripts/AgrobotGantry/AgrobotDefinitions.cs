using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgrobotDefinitions : MonoBehaviour
{
    [SerializeField] private float m_movementSpeed = 1;
    public float MovementSpeed { get => m_movementSpeed; } 

    [SerializeField] private float m_turningSpeed = 1;
    public float TurningSpeed { get => m_turningSpeed; }


    [SerializeField] private float m_equipmentSpeed = 1;
    public float EquipmentSpeed { get => m_equipmentSpeed; }


    private static AgrobotDefinitions s_instance = null;
    public static AgrobotDefinitions Instance { get => s_instance; }

    private void Start()
    {
        if (s_instance != null)
        {
            Debug.LogError("Can only have 1 AgrobotDefinitions behaviour in a scene!");
            return;
        }

        s_instance = this;
    }
}
