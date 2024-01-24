using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    private DefaultPlayerInput _defaultPlayerInput;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private Rigidbody _rigidbody;
    private Transform _transform;
    private Transform _cameraTransform;

    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _mouseSensitivity = 10f;
    private float verticalAngle = 0f; // Vertical angle of the camera
    private float horizontalAngle = 0f; // Horizontal angle of the camera
    private bool _hasCameraTransitioned = false;
    
    
    
    // Handling camera movement next to the door
    private bool isNearDoor = false;
    private Vector3 closeCameraOffset = new Vector3(0, 0, -1);
    private Vector3 initialCameraPosition;
    [SerializeField] private Transform doorTransform; // Assign this in the Inspector
    private float enterDoorProximityDistance = 2f; // Distance at which the camera starts moving closer
    private float exitDoorProximityDistance = 3f;  // Distance at which the camera moves back

   

    private void Awake()
    {
        _defaultPlayerInput = new DefaultPlayerInput();
        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
        _cameraTransform = Camera.main.transform;
        initialCameraPosition = _cameraTransform.position;
    }

    private void OnEnable()
    {
        _moveAction = _defaultPlayerInput.Player.Move;
        _moveAction.Enable();
        _lookAction = _defaultPlayerInput.Player.Look;
        _lookAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _lookAction.Disable();
    }
    
    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
        CheckProximityToDoor();
        
    }
    
    
    
    private void HandleMovement()
    {
        // Player movements
        Vector2 moveInput = _moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

        if (isNearDoor)
        {
            // When near the door, the camera follows the player but doesn't rotate
            moveDirection = _transform.forward * moveDirection.z + _transform.right * moveDirection.x;
            moveDirection.y = 0; // Keep the player grounded
        }
        else
        {
            // Align movement direction to camera's orientation
            moveDirection = _cameraTransform.forward * moveDirection.z + _cameraTransform.right * moveDirection.x;
            moveDirection.y = 0; // Keep the player grounded
        }

        // Apply movement
        Vector3 velocity = moveDirection.normalized * _speed;
        _rigidbody.velocity = new Vector3(velocity.x, _rigidbody.velocity.y, velocity.z);

        // Optionally, make the character face the direction of movement
        if (!isNearDoor && moveDirection.magnitude > 0.1f)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            _transform.rotation = Quaternion.Slerp(_transform.rotation, toRotation, 10f * Time.deltaTime);
        }
    }

    

    private void CheckProximityToDoor()
    {
        float distanceToDoor = Vector3.Distance(_transform.position, doorTransform.position);

        if (isNearDoor)
        {
            // Check if the player has moved beyond the exit threshold
            if (distanceToDoor > exitDoorProximityDistance)
            {
                isNearDoor = false;
            }
        }
        else
        {
            // Check if the player is within the enter threshold
            if (distanceToDoor < enterDoorProximityDistance)
            {
                isNearDoor = true;
            }
        }
    }

    private void HandleRotation()
    {
        if (isNearDoor)
        {
            // Calculate the direction vector from the player to the door
            Vector3 doorDirection = doorTransform.position - _transform.position;
            doorDirection.y = 0f; // Ensure it's on the same horizontal plane

            // Calculate the perpendicular direction (90 degrees to the door direction)
            Vector3 perpendicularDirection = Quaternion.Euler(0, 0, 90) * doorDirection;

            // Gradually update the player's forward direction to align with the perpendicular direction
            Vector3 newForwardDirection = Vector3.Slerp(_transform.forward, perpendicularDirection.normalized, Time.deltaTime * 5f);

            // Apply the new forward direction to the player's rotation
            _transform.rotation = Quaternion.LookRotation(newForwardDirection);

            // Offset the camera position from the player's position
            Vector3 cameraOffset = _transform.forward * -2f; // Adjust the offset distance as needed
            Vector3 cameraPosition = _transform.position + cameraOffset;

            // Gradually update the camera's position to smooth the transition
            _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, cameraPosition, Time.deltaTime * 5f);

            // Ensure the camera looks at the player
            _cameraTransform.LookAt(_transform.position);
          

        }
        else
        {
            // Update the initial camera position relative to the player's current orientation
            initialCameraPosition =
                _transform.position - _transform.forward * 3f + Vector3.up; // Adjust the Vector3.up as needed

            // Smoothly transition back to the initial camera position
            _cameraTransform.position =
                Vector3.Lerp(_cameraTransform.position, initialCameraPosition, Time.deltaTime * 5f);
            
            Vector2 lookInput = _lookAction.ReadValue<Vector2>();
            float mouseX = lookInput.x * _mouseSensitivity * Time.deltaTime;
            float mouseY = lookInput.y * _mouseSensitivity * Time.deltaTime;

            // Update the horizontal and vertical angles based on the input
            horizontalAngle += mouseX;
            verticalAngle -= mouseY; 
            verticalAngle = Mathf.Clamp(verticalAngle, 0f, 25f); // Limit vertical angle

            // Calculate the new rotation and position for the camera
            Quaternion rotation = Quaternion.Euler(2, horizontalAngle, 0);
            Vector3 direction = rotation * -Vector3.forward;
            Vector3 cameraPosition = _transform.position + direction * 3f;

            // Apply the rotation and position to the camera
            _cameraTransform.rotation = rotation;
            _cameraTransform.position = cameraPosition;   
        }
    }
}