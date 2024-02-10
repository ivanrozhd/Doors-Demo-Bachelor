using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Separate class for the dynamic part of the gaming door - explicit for the dragging hinged doors
public class DraggableDoor : MonoBehaviour
{ 
    private HingeJoint _hinge;
    private Vector3 _initialMousePosition;
    private float _initialAngle;
    public float maxOpenAngle = 90f; // Maximum opening angle
    public float force = 10f; // Force to apply for opening/closing the door

    void Start()
    {
        _hinge = GetComponent<HingeJoint>();
        _hinge.useSpring = true; // Enable spring to smoothly stop the door
    }

    void OnMouseDown()
    {
        // Record initial mouse position
        _initialMousePosition = Input.mousePosition;
        _initialAngle = _hinge.angle; // Assuming you have an angle property or similar to get the current hinge rotation
    }
    
    
    void OnMouseDrag()
    {
        Vector3 delta = Input.mousePosition - _initialMousePosition;
        float targetAngle = Mathf.Clamp(_initialAngle + (delta.x * force), -maxOpenAngle, 0f);

        // Set the target angle within the limits
        JointSpring spring = _hinge.spring;
        spring.targetPosition = targetAngle;
        _hinge.spring = spring;
    }
    
}


