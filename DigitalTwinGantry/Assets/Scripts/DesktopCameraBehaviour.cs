using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This script will handle all the keyboard and mouse input to navigate through the application in desktop mode
/// </summary>
public class DesktopCameraBehaviour : MonoBehaviour
{
    [SerializeField] private float m_rotationSpeed = 0.2f;
    [SerializeField] private float m_movementSpeed = 0.05f;

    private bool m_held, m_mouseHeld = false;
    private Vector3 m_direction;
    private Vector3 m_rotation, m_position;
    private Vector2 m_mousePosition;

    private void Start()
    {
        m_rotation = transform.eulerAngles;
        m_position = transform.position;
    }

    void Update()
    {
        if (m_mouseHeld)
        {
            m_rotation.y += m_mousePosition.y;
            m_rotation.x -= m_mousePosition.x;
           transform.eulerAngles = m_rotation;
        }
        
        if (m_held)
        {
            float dx = m_direction.z * Mathf.Sin((m_rotation.y ) * Mathf.Deg2Rad);
            float dz = m_direction.z * Mathf.Cos((m_rotation.y) * Mathf.Deg2Rad);
            dx += m_direction.x * Mathf.Sin((m_rotation.y +90)* Mathf.Deg2Rad);
            dz += m_direction.x * Mathf.Cos((m_rotation.y +90)* Mathf.Deg2Rad);

            m_position.x += dx;
            m_position.z += dz;
            m_position.y += m_direction.y;

            if (transform.position.y < 0.5)
            {
                m_position.y = 0.5f;
            }
            transform.position = m_position;
        }

    }
    
    public void OnMove(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            m_direction = input.ReadValue<Vector3>() * m_movementSpeed * Time.deltaTime;
            m_held = true;
        }
        if (input.canceled)
        {
            m_held = false;
        }
    }

    public void OnLook(InputAction.CallbackContext input)
    {
        m_mousePosition.y = input.ReadValue<Vector2>().x * m_rotationSpeed * Time.deltaTime;
        m_mousePosition.x = input.ReadValue<Vector2>().y * m_rotationSpeed * Time.deltaTime;
    }

    public void OnRotate(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            m_mouseHeld = true;
        }

        if (input.canceled)
        {
            m_mouseHeld = false;
        }
    }
}
