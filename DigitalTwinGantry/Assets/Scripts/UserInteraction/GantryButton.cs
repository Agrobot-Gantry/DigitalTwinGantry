using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This button is made specifically for this program but is nothing really more than just a button.
/// </summary>
public class GantryButton : MonoBehaviour
{
    [SerializeField] private string m_interactorTag;
    [SerializeField] private float m_yMovement;
    [SerializeField] private AudioSource m_audio;
    [SerializeField] private UnityEvent m_onClick;

    private bool m_isPressed = false;

    public bool IsPressed => m_isPressed;

    private float m_startY;
    private float m_offsetY;

    private bool m_first = true;
    private Collider m_interactor = null;

    private void Start() 
    {
        m_startY = transform.position.y;
    }

    private void Update() 
    {
        if (m_interactor == null) 
        {
            m_first = true;
            transform.position = new Vector3(transform.position.x, m_startY, transform.position.z);
            return;
        }

        if (m_first) 
        {
            m_first = false;
            m_offsetY = transform.position.y -  m_interactor.transform.position.y;
        }

        float interactorY = m_interactor.transform.position.y + m_offsetY;
        interactorY = Mathf.Clamp(interactorY, m_startY - m_yMovement, m_startY);

        if (interactorY < m_startY - m_yMovement + 0.001) 
        {
            if (m_isPressed == false) 
            {
                m_audio?.Play();
                m_onClick.Invoke();
            }

            m_isPressed = true;
        } else 
        {
            m_isPressed = false;
        }

        transform.position = new Vector3(transform.position.x, interactorY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.tag == m_interactorTag) 
        {
            m_interactor = other;
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (m_interactor == other) 
        {
            m_interactor = null;
        }
    }
}
