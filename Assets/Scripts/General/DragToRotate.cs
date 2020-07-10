using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class DragToRotate : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 1f;
    Vector2 previousMousePos = Vector2.zero;
    Vector2 newMousePos = Vector2.zero;
    // The rotation that will be applied to the object if the user is rotating it
    Vector3 rotation = Vector3.zero;
    // Set to true when the user is rotating the object (mouse is held down)
    bool isMouseHeld = false;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray;
            RaycastHit hit;
            // Raycast to make sure we hit the object that we want to rotate, which is the object this script is attached to
            ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.name == gameObject.name)
                {
                    // The ray hit our current object! start rotating it
                    previousMousePos = Mouse.current.position.ReadValue();
                    isMouseHeld = true;
                }
            }
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame && isMouseHeld)
        {
            // The user released the mouse, stop rotating
            isMouseHeld = false;
        }

        if (isMouseHeld)
        {
            // If the mouse position changed, rotate the object by the delta position
            newMousePos = Mouse.current.position.ReadValue();
            if (newMousePos != previousMousePos)
            {
                Vector2 offset = newMousePos - previousMousePos;
                // Rotate along y axis only
                rotation.x = transform.rotation.x;
                rotation.y = -(offset.x + offset.y) * rotationSpeed;
                transform.Rotate(rotation);
                previousMousePos = newMousePos;
            }
        }
    }
}
