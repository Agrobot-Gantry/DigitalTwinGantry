using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Oculus.Interaction;

/// <summary>
/// This script is used to generate sections in a slider and measure at which section the slider is.
/// </summary>
public class SliderMeter : MonoBehaviour
{
    [SerializeField] private GameObject m_divider;
    [SerializeField] private Grabbable m_grabbable;
    [SerializeField] private OneGrabTranslateTransformer m_transformer;
    [SerializeField] private int m_totalSelections;
    [SerializeField] private AudioSource m_audio;
    [SerializeField] private UnityEvent<int> m_onSelectionChanged;

    private float[] m_sections;

    private int m_currentSection;
    public int CurrentSection => m_currentSection;
    public int TotalSections => m_totalSelections;

    private void Start() 
    {
        m_grabbable.WhenGrabbableUpdated += WhenGrabbableUpdated;

        float divider = 1f / m_totalSelections;
        float linear = 0f;

        m_sections = new float[m_totalSelections];

        for (int i = 0; i < m_totalSelections; i++) 
        {
            if (i == 0) 
            {
                linear = divider / 2;
            } else 
            {
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

        CalculateCurrentSection(true);
    }

    private void WhenGrabbableUpdated(GrabbableArgs args) 
    {
        CalculateCurrentSection(false);
    }

    private void CalculateCurrentSection(bool isReset) 
    {
        float smallestDistance = 1000;

        int previousSection = m_currentSection;

        for (int i = 0; i < m_sections.Length; i++) 
        {
            float distance = Mathf.Abs(transform.localPosition.x - m_sections[i]);

            if (distance < smallestDistance) 
            {
                m_currentSection = i;
                smallestDistance = distance;
            }
        }

        if (previousSection != m_currentSection && !isReset) 
        {
            m_audio?.Play();
            m_onSelectionChanged.Invoke(CurrentSection);
        }
    }

    private void OnDestroy() 
    {
        m_grabbable.WhenGrabbableUpdated -= WhenGrabbableUpdated;
    }    

    private void OnValidate() 
    {
        m_totalSelections = Mathf.Max(m_totalSelections, 2);
    }
}
