using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DesktopCameraBehaviour : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 0.2f;
    [SerializeField]
    private float movementSpeed = 0.05f;

    private bool held, mouseHeld = false;
    private Vector3 direction;
    Vector3 rotation, position;
    Vector2 mousePosition;

    private void Start()
    {
       rotation = transform.eulerAngles;
        position = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
      
        if (mouseHeld)
        {
            rotation.y += mousePosition.y;
            rotation.x -= mousePosition.x;
           transform.eulerAngles = rotation;
        }
        if (held)
        {
            float dx = direction.z * Mathf.Sin((rotation.y ) * Mathf.Deg2Rad);
            float dz = direction.z * Mathf.Cos((rotation.y) * Mathf.Deg2Rad);
            dx += direction.x * Mathf.Sin((rotation.y +90)* Mathf.Deg2Rad);
            dz += direction.x * Mathf.Cos((rotation.y +90)* Mathf.Deg2Rad);

            position.x += dx;
            position.z += dz;
            position.y += direction.y;

            if (transform.position.y < 0.5)
            {
                position.y = 0.5f;
            }
            transform.position = position;
        }

    }

    public void onMove(InputAction.CallbackContext input)
    {
        
        if (input.performed)
        {
            direction = input.ReadValue<Vector3>() * movementSpeed * Time.deltaTime;
            held = true;
        }
        if (input.canceled)
        {
            held = false;
        }
    }
    public void onLook(InputAction.CallbackContext input)
    {
        
        mousePosition.y = input.ReadValue<Vector2>().x * rotationSpeed * Time.deltaTime;
        mousePosition.x = input.ReadValue<Vector2>().y * rotationSpeed * Time.deltaTime;

    }

    public void onRotate(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            mouseHeld = true;
        }
        if (input.canceled)
        {
            mouseHeld = false;
        }
    }
}
