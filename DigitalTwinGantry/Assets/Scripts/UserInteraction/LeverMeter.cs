using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Oculus.Interaction;

/// <summary>
/// This script is used to measure the value of a lever and make it accessible
/// </summary>
public class LeverMeter : MonoBehaviour
{
    [SerializeField] private Grabbable m_grabbable;
    [SerializeField] private OneGrabRotateTransformer m_transformer;
    [SerializeField] private OneGrabRotateTransformer.Axis m_axis;
    [SerializeField] private UnityEvent<float> m_onUpdate;

    private float m_value;
    public float Value => m_value;

    private void Start() 
    {
        m_grabbable.WhenGrabbableUpdated += WhenGrabbableUpdated;
    }

    /// <summary>
    /// Maps a value from one range to another
    /// </summary>
    /// <param name="from">The value that needs to be remapped</param>
    /// <param name="fromMin">The minimal possible value of <see cref="from"/></param>
    /// <param name="fromMax">The maximal possible value of <see cref="from"/></param>
    /// <param name="toMin">The new minumum possible value</param>
    /// <param name="toMax">The new maximum possible value</param>
    /// <returns>A remapped value</returns>
    public static float Remap (float from, float fromMin, float fromMax, float toMin,  float toMax)
    {
        var fromAbs  =  from - fromMin;
        var fromMaxAbs = fromMax - fromMin;      
       
        var normal = fromAbs / fromMaxAbs;
 
        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;
 
        var to = toAbs + toMin;
       
        return to;
    }

    private void WhenGrabbableUpdated(GrabbableArgs args) 
    {
        if (args.GrabbableEvent == GrabbableEvent.Update) 
        {
            float rot = 0;
            switch (m_axis) 
            {
                case OneGrabRotateTransformer.Axis.Right: rot = m_grabbable.transform.eulerAngles.x; break;
                case OneGrabRotateTransformer.Axis.Up: rot = m_grabbable.transform.eulerAngles.y; break;
                case OneGrabRotateTransformer.Axis.Forward: rot = m_grabbable.transform.eulerAngles.z; break;
            }

            if (rot > 180) 
            {
                rot = -(360 - rot);
            }

            m_value = Remap(rot, m_transformer.Constraints.MinAngle.Value, m_transformer.Constraints.MaxAngle.Value, 0, 1);

            m_onUpdate.Invoke(Value);
        }
    }

    private void OnDestroy() 
    {
        m_grabbable.WhenGrabbableUpdated -= WhenGrabbableUpdated;
    }
}
