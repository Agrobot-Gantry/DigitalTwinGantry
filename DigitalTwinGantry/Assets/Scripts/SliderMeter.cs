using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class SliderMeter : MonoBehaviour
{
    [SerializeField] private GameObject m_divider;
    [SerializeField] private Grabbable m_grabbable;
    [SerializeField] private OneGrabTranslateTransformer m_transformer;
    [SerializeField] private int m_totalSelections;

    private float[] m_sections;

    private int m_currentSection;
    public int CurrentSection { get => m_currentSection; }

    private void Start() {
        m_grabbable.WhenGrabbableUpdated += WhenGrabbableUpdated;

        float divider = 1f / m_totalSelections;
        float linear = 0f;

        m_sections = new float[m_totalSelections];

        for (int i = 0; i < m_totalSelections; i++) {
            if (i == 0) {
                linear = divider / 2;
            } else {
                linear += divider;
            }

            m_sections[i] = Mathf.Lerp(m_transformer.Constraints.MinX.Value, m_transformer.Constraints.MaxX.Value, linear);

            if (i < m_totalSelections - 1)
            {
                var obj = Instantiate(m_divider, transform.parent);
                float x = Mathf.Lerp(m_transformer.Constraints.MinX.Value, m_transformer.Constraints.MaxX.Value, linear + (divider / 2));
                obj.transform.localPosition = new Vector3(x, 0, 0);
            }
        }

        CalculateCurrentSection();
    }

    private void WhenGrabbableUpdated(GrabbableArgs args) {
        if (args.GrabbableEvent == GrabbableEvent.Remove) {
            CalculateCurrentSection();
            Debug.Log(CurrentSection);
        }
    }

    private void CalculateCurrentSection() {
        float smallestDistance = 1000;
        for (int i = 0; i < m_sections.Length; i++) {
            float distance = Mathf.Abs(transform.position.x - m_sections[i]);

            if (distance < smallestDistance) {
                m_currentSection = i;
                smallestDistance = distance;
            }
        }
    }

    private void OnDestroy() {
        m_grabbable.WhenGrabbableUpdated -= WhenGrabbableUpdated;
    }    

    private void OnValidate() {
        m_totalSelections = Mathf.Max(m_totalSelections, 2);
    }
}
