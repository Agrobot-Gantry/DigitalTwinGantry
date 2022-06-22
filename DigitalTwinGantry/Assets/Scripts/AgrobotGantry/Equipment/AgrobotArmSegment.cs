using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

/// <summary>
/// A single segment of the total agrobot arm.
/// This class uses inversed kinematics to reach for points.
/// </summary>
public class AgrobotArmSegment : MonoBehaviour
{
    [SerializeField] private Transform m_hingePoint;
    /// <summary>
    /// The lower hinge point of the model of the segment.
    /// </summary>
    public Vector3 HingePoint => m_hingePoint.position;

    [SerializeField] private Transform m_endPos;
    /// <summary>
    /// The lowest end point of the model of the segment.
    /// </summary>
    public Vector3 EndPos => m_endPos.position;

    /// <summary>
    /// Moves and rotates the segment so the EndPos reaches the target.
    /// </summary>
    /// <param name="target">The point the segment needs to reach</param>
    public void Reach(Vector3 target)
    {
        Vector3 direction = target - transform.position;

        float value = target.x - transform.position.x;
        if (value == 0)
        {
            value = 0.001f;
        }

        float yRotation = Mathf.Atan((target.z - transform.position.z) / value);

        float sinTheta = Mathf.Sin(-yRotation);
        float cosTheta = Mathf.Cos(-yRotation);
            
        Vector3 rotatedTarget = target;

        float x = rotatedTarget.x - transform.position.x;
        float z = rotatedTarget.z - transform.position.z;

        rotatedTarget.x = x * cosTheta - z * sinTheta + transform.position.x;
        rotatedTarget.z = z * cosTheta + x * sinTheta + transform.position.z;

        Vector3 angle = GetAngle(transform.position, rotatedTarget);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, Mathf.Rad2Deg * -yRotation, angle.z);

        direction.Normalize();
        direction *= transform.lossyScale.y;
        direction = -direction;

        transform.position = target + direction;
    }

    /// <summary>
    /// Calculates the 3D rotation (Euler) needed to rotate from one vector to the other 
    /// </summary>
    /// <param name="from">Start vector</param>
    /// <param name="to">The vector the <see cref="from"/> vector needs to rotate to</param>
    /// <returns>The rotation (Euler) needed to rotate from the <see cref="from"/> vector to the <see cref="to"/> vector</returns>
    private static Vector3 GetAngle(Vector3 from, Vector3 to)
    {
        float rotX = Mathf.Rad2Deg * Mathf.Atan2(to.z - from.z, to.y - from.y);
        float rotY = Mathf.Rad2Deg * Mathf.Atan2(to.z - from.z, to.x - from.x);
        float rotZ = Mathf.Rad2Deg * -Mathf.Atan2(to.x - from.x, to.y - from.y);

        return new Vector3(rotX, rotY, rotZ);
    }
}
