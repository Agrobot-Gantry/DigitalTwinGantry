using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class InKinematicSegment : MonoBehaviour
{
    [SerializeField] private Transform m_endPos;

    public Vector3 EndPos { get => m_endPos.position; }

    public void Follow(Vector3 target)
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
        direction = -direction;

        transform.position = target + direction;
    }

    // public void CalculateEndPos()
    // {
    //     // float dx = transform.lossyScale.y * Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.z) * Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.y);
    //     float dy = transform.lossyScale.y * Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.z);
    //     // float dz = transform.lossyScale.y * Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.z) * Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.y);
    //
    //     EndPos = new Vector3(transform.position.x, transform.position.y - dy, transform.position.z);
    // }

    public static Vector3 GetAngle(Vector3 from, Vector3 to)
    {
        float rotX = Mathf.Rad2Deg * Mathf.Atan2(to.z - from.z, to.y - from.y);
        float rotY = Mathf.Rad2Deg * Mathf.Atan2(to.z - from.z, to.x - from.x);
        float rotZ = Mathf.Rad2Deg * -Mathf.Atan2(to.x - from.x, to.y - from.y);

        return new Vector3(rotX, rotY, rotZ);
    }
}
