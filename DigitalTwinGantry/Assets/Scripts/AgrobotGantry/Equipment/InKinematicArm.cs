using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InKinematicArm : MonoBehaviour
{
    [SerializeField] private Vector3 point = new Vector3(0, -5, 0);

    [SerializeField] private GameObject m_segmentPrefab;
    [SerializeField] private int m_totalSegments;

    private List<InKinematicSegment> m_segments;

    private void Start()
    {
        m_segments = new List<InKinematicSegment>();
        float addY = 0;
        for (int i = 0; i < m_totalSegments; i++)
        {
            GameObject segment = Instantiate(m_segmentPrefab, transform);
            segment.transform.localPosition = new Vector3(segment.transform.localPosition.x, segment.transform.localPosition.y + addY, segment.transform.localPosition.z);
            m_segments.Add(segment.GetComponent<InKinematicSegment>());

            addY += segment.transform.localScale.y;
        }
    }

    private void Update()
    {
        ReachForPoint(point);
    }

    private void ReachForPoint(Vector3 point)
    {
        InKinematicSegment end = m_segments[m_segments.Count - 1];
        end.Follow(point);
        end.CalculateEndPos();

        for (int i = m_segments.Count - 2; i >= 0; i--)
        {
            m_segments[i].Follow(m_segments[i + 1].transform.position);
            m_segments[i].CalculateEndPos();
        }
    }
}
