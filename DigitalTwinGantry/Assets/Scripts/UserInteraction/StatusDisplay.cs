using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusDisplay : MonoBehaviour
{
    [SerializeField] private Renderer[] m_displays;
    [SerializeField] private Material m_activeColor;
    [SerializeField] private Material m_inactiveColor;
    [SerializeField] private Material m_disabledColor;
    
    public void SetStatus(bool? active)
	{
        switch(active)
		{
            case true:
                foreach (Renderer renderer in m_displays)
				{
                    renderer.material = m_activeColor;
				}
                break;
            case false:
                foreach (Renderer renderer in m_displays)
                {
                    renderer.material = m_inactiveColor;
                }
                break;
            case null:
                foreach (Renderer renderer in m_displays)
                {
                    renderer.material = m_disabledColor;
                }
                break;
		}
	}
}
