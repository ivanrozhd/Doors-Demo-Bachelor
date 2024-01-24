using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableDoor : MonoBehaviour
{ 
    private HingeJoint hinge;
    private Vector3 initialMousePosition;
    private float initialAngle;
    public float maxOpenAngle = 90f; // Maximum opening angle
    public float force = 10f; // Force to apply for opening/closing the door

    void Start()
    {
        hinge = GetComponent<HingeJoint>();
        hinge.useSpring = true; // Enable spring to smoothly stop the door
    }

    void OnMouseDown()
    {
        // Record initial mouse position
        initialMousePosition = Input.mousePosition;
        initialAngle = hinge.angle; // Assuming you have an angle property or similar to get the current hinge rotation
    }
    
    
    void OnMouseDrag()
    {
        Vector3 delta = Input.mousePosition - initialMousePosition;
        float targetAngle = Mathf.Clamp(initialAngle + (delta.x * force), -maxOpenAngle, 0f);

        // Set the target angle within the limits
        JointSpring spring = hinge.spring;
        spring.targetPosition = targetAngle;
        hinge.spring = spring;
    }

}


