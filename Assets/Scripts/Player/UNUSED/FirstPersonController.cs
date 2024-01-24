using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    private DefaultPlayerInput _defaultPlayerInput;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private Rigidbody _rigidbody;
    private Transform _transform;
    private Transform _cameraTransform;
    private float _xRotation = 0f;
    private Dictionary<string, IDoorBehavior> _doorBehaviors;

    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _mouseSensitivity = 10f;

    private void Awake()
    {
        _defaultPlayerInput = new DefaultPlayerInput();
        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
        _cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
        Cursor.lockState = CursorLockMode.Locked;
        InitializeDoorBehaviors();
    }
    
    
    
    private void InitializeDoorBehaviors()
    {
        _doorBehaviors = new Dictionary<string, IDoorBehavior>
        {
            {"Door0", GameObject.FindGameObjectWithTag("Door0").GetComponent<Behaviour0>()},
            {"Door1", GameObject.FindGameObjectWithTag("Door1").GetComponent<Behaviour1>()},
            {"Door5", GameObject.FindGameObjectWithTag("Door5").GetComponent<Behaviour1>()},
            {"Door6", GameObject.FindGameObjectWithTag("Door6").GetComponent<Behaviour67>()},
            {"Door7", GameObject.FindGameObjectWithTag("Door7").GetComponent<Behaviour67>()},
            {"Door8", GameObject.FindGameObjectWithTag("Door8").GetComponent<Behaviour8>()},
            {"Door9", GameObject.FindGameObjectWithTag("Door9").GetComponent<Behaviour9>()},
        };
    }


    private void OnEnable()
    {
        // moving actions
        _moveAction = _defaultPlayerInput.Player.Move;
        _moveAction.Enable();
        
        // looking actions
        _lookAction = _defaultPlayerInput.Player.Look;
        _lookAction.Enable();
        
        // press buttom on the keyboard
        _defaultPlayerInput.Player.Open.performed += OnOpenAction;
        _defaultPlayerInput.Player.Open.Enable();
        
        // press mouse button
        _defaultPlayerInput.Player.Click.performed += OnClick;
        _defaultPlayerInput.Player.Click.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _lookAction.Disable();
        _defaultPlayerInput.Player.Open.performed -= OnOpenAction;
        _defaultPlayerInput.Player.Open.Disable();
        _defaultPlayerInput.Player.Click.performed -= OnClick;
        _defaultPlayerInput.Player.Click.Disable();
    }
    
    // Method to open the door when the "Open" action is performed
    private void OnClick(InputAction.CallbackContext context)
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Interactable"))
                {
                    // Call the interact method on the hit object
                    // hit.collider.GetComponent<ButtonInteraction>().Interact();
                    Debug.Log("Interacted");    
                    if (!_doorBehaviors["Door5"].IsOpening)
                    {
                        Debug.Log("Interacted");  
                        // _doorBehavior2.OpenDoor();
                        _doorBehaviors["Door5"].OpenDoor();
                    }
                }
                if (hit.collider.CompareTag("Key6"))
                {
                    Debug.Log("Interacted 6");
                    _doorBehaviors["Door6"].TakeKey(hit.collider.gameObject);
                }
                
                if (hit.collider.CompareTag("Key7"))
                {
                    Debug.Log("Interacted 7");
                    _doorBehaviors["Door7"].TakeKey(hit.collider.gameObject);
                }
                
                if (hit.collider.CompareTag("obstacle"))
                {
                    Debug.Log("obstacle");
                    _doorBehaviors["Door8"].DestroyObstacle(hit.collider.gameObject);
                }
            }   
        }
    }
    
    
    // Method to open the door when the "Open" action is performed "E pressed"
    private void OnOpenAction(InputAction.CallbackContext context)
    {
        foreach (var kvp in _doorBehaviors)
        {
            if (kvp.Value.NextToDoor && !kvp.Value.IsOpening && kvp.Value.KeyPicked && kvp.Value.NoObstacleExist && !kvp.Value.Opened)
            {
                kvp.Value.OpenDoor();
                break; 
            }
            
            if (kvp.Value.NextToDoor && !kvp.Value.IsOpening && kvp.Value.KeyPicked && kvp.Value.NoObstacleExist && kvp.Value.Opened)
            {
                kvp.Value.CloseDoor();
                break; 
            }
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }
    
    private void HandleMovement()
    {
        // player movements
        Vector2 moveDir = _moveAction.ReadValue<Vector2>();
        Vector3 velocity = _rigidbody.velocity;
        Vector3 moveDirection = _transform.forward * moveDir.y + _transform.right * moveDir.x;
        moveDirection.Normalize();
        velocity.x = moveDirection.x * _speed;
        velocity.z = moveDirection.z * _speed;
        _rigidbody.velocity = velocity;
    }

    private void HandleRotation()
    {
        //Player Rotation
        Vector2 lookDir = _lookAction.ReadValue<Vector2>();
        float mouseX = lookDir.x * _mouseSensitivity * Time.deltaTime;
        float mouseY = lookDir.y * _mouseSensitivity * Time.deltaTime;
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 25f);
        _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        _transform.Rotate(Vector3.up * mouseX);
    }
    
    
    
}
