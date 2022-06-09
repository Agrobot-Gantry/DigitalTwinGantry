using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgrobotArm : MonoBehaviour
{
    [Header("Model settings")]
    [SerializeField] private GameObject m_segmentPrefab;
    [SerializeField] private Material m_segmentMaterial;
    [SerializeField] private GameObject m_toolEffect;

    [Header("Transform settings")]
    [SerializeField] private Vector3 m_reachPoint;
    [SerializeField] private Transform m_basePoint;
    [SerializeField] private Transform m_restPoint;
    [SerializeField] private int m_totalSegments;
    [SerializeField] private bool m_isAttached;

    private List<AgrobotArmSegment> m_segments;
    private Vector3 m_currentReachPoint;

    private AgrobotToolEffect m_effect;

    private static bool s_busy = false;

    private void Start()
    {
        m_segments = new List<AgrobotArmSegment>();
        for (int i = 0; i < m_totalSegments; i++)
        {
            GameObject segment = Instantiate(m_segmentPrefab, transform);
            Renderer[] renderers = segment.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
            {
                r.material = m_segmentMaterial;
            }

            segment.transform.localPosition = new Vector3(segment.transform.localPosition.x, segment.transform.localPosition.y, segment.transform.localPosition.z);
            m_segments.Add(segment.GetComponent<AgrobotArmSegment>());
        }

        if (m_toolEffect != null)
        {
            GameObject toolEffect = Instantiate(m_toolEffect, m_segments[m_segments.Count - 1].gameObject.transform);
            toolEffect.transform.position = m_segments[m_segments.Count - 1].EndPos;
            m_effect = toolEffect.GetComponentInChildren<AgrobotToolEffect>();
        }

        m_currentReachPoint = transform.position + m_reachPoint;
        ReachForPointInstant(m_currentReachPoint);
        ReachForPointInstant(m_restPoint.position);
    }

    public void NeutralPosition(float speed)
    {
        StartCoroutine(ReachForPointSmooth(m_restPoint, 0.5f, speed, false));
    }

    public IEnumerator ReachForPointSmooth(Transform point, float minDistance, float speed, bool doEffect = true)
    {
        while (s_busy)
        {
            yield return null;
        }

        s_busy = true;

        ResetReach();
        while (Vector3.Distance(m_currentReachPoint, point.position) > minDistance)
        {
            m_currentReachPoint = Vector3.MoveTowards(m_currentReachPoint, point.position, speed * TimeChanger.DeltaTime);
            m_currentReachPoint = Vector3.Lerp(m_currentReachPoint, point.position, speed * TimeChanger.DeltaTime);

            ReachForPointInstant(m_currentReachPoint);

            yield return null;
        }

        if (m_effect != null && doEffect)
        {
            m_effect.OEffectStart();
        }

        s_busy = false;
    }

    public void ReachForPointInstant(Vector3 point)
    {
        AgrobotArmSegment end = m_segments[m_segments.Count - 1];
        end.Follow(point);

        for (int i = m_segments.Count - 2; i >= 0; i--)
        {
            m_segments[i].Follow(m_segments[i + 1].transform.position);
        }

        if (!m_isAttached)
        {
            return;
        }

        m_segments[0].gameObject.transform.position = m_basePoint.position;
        for (int i = 1; i < m_segments.Count; i++)
        {
            m_segments[i].gameObject.transform.position = m_segments[i - 1].EndPos;
        }
    }

    private void ResetReach()
    {
        m_currentReachPoint = m_segments[m_segments.Count - 1].EndPos;
    }

    private void OnValidate()
    {
        if (m_toolEffect != null)
        {
            if (m_toolEffect.GetComponentInChildren<AgrobotToolEffect>() == null)
            {
                Debug.LogError("Tool effect gameobject should contain a AgrobotToolEffect component");
                m_toolEffect = null;
            }
        }
    }
}
