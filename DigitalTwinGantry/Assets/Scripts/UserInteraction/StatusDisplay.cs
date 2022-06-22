using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusDisplay : MonoBehaviour
{
    [SerializeField] private Renderer[] m_displays;
    [SerializeField] private Image[] m_imageDisplays;
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
                foreach (Image image in m_imageDisplays)
				{
                    image.color = m_activeColor.color;
                }
                break;
            case false:
                foreach (Renderer renderer in m_displays)
                {
                    renderer.material = m_inactiveColor;
                }
                foreach (Image image in m_imageDisplays)
                {
                    image.color = m_inactiveColor.color;
                }
                break;
            case null:
                foreach (Renderer renderer in m_displays)
                {
                    renderer.material = m_disabledColor;
                }
                foreach (Image image in m_imageDisplays)
                {
                    image.color = m_disabledColor.color;
                }
                break;
		}
	}
}
