using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// First player controller: moving, looking and interaction with gaming doors, teleporting
public class FPC : MonoBehaviour, IDoorObserver
{
    
    // parameters for the player signals and actions
    private DefaultPlayerInput _defaultPlayerInput;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private Rigidbody _rigidbody;
    private Transform _transform;
    private Transform _cameraTransform;
    private float _xRotation = 0f;
    private Vector3 _initialPosition;
   
    
    //Extension of gaming doors to make sure that the player can interact with all of them
    private List<IDoorBehavior> _doors; 

    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _mouseSensitivity = 10f;

    private void Awake()
    {
        _defaultPlayerInput = new DefaultPlayerInput();
        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
        _cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
        Cursor.lockState = CursorLockMode.Locked;
        _initialPosition = this.gameObject.transform.position;
        _doors = new List<IDoorBehavior>();
        DoorCustomizationUI doorCustomizationUI = FindObjectOfType<DoorCustomizationUI>();
        if (doorCustomizationUI != null)
        {
            doorCustomizationUI.Subscribe(this);
        }
        else
        {
            Debug.LogWarning("DoorCustomizationUI not found in the scene.");
            
        }
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
        
        // press mouse button
        _defaultPlayerInput.Player.Teleport.performed += Teleport;
        _defaultPlayerInput.Player.Teleport.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _lookAction.Disable();
        _defaultPlayerInput.Player.Open.performed -= OnOpenAction;
        _defaultPlayerInput.Player.Open.Disable();
        _defaultPlayerInput.Player.Click.performed -= OnClick;
        _defaultPlayerInput.Player.Click.Disable();
        _defaultPlayerInput.Player.Teleport.performed -= Teleport;
        _defaultPlayerInput.Player.Teleport.Disable();
        
        DoorCustomizationUI doorCustomizationUI = FindObjectOfType<DoorCustomizationUI>();
        if (doorCustomizationUI != null)
        {
            doorCustomizationUI.Unsubscribe(this);
        }
    }
    
    
    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }
    
    
    
    // check out when the new door is initialized in the scene
    public void InitializeDoorBehaviors(GameObject door)
    {
        IDoorBehavior doorBehavior = door.GetComponent<IDoorBehavior>();
        if (doorBehavior != null && !_doors.Contains(doorBehavior))
        {
                    _doors.Add(doorBehavior); 
        }
    }

    // return to the door generator
    private void Teleport(InputAction.CallbackContext context)
    {
        this.gameObject.transform.position = _initialPosition;
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
                Transform currentTransform = hit.collider.transform;
                if (hit.collider.CompareTag("Interactable"))
                {
                    // Traverse up the hierarchy and check for the IDoorBehavior component
                    IDoorBehavior doorBehavior = null;
                    while (currentTransform != null && doorBehavior == null)
                    {
                        doorBehavior = currentTransform.GetComponent<IDoorBehavior>();
                        currentTransform = currentTransform.parent;
                    }

                    if (doorBehavior != null)
                    {
                        if (doorBehavior.Interaction.Equals(DoorBehaviorTriggerType.Button) && !doorBehavior.Opened && doorBehavior.NoObstacleExist)
                        {
                            doorBehavior.OpenDoor();
                        }
                        else if (doorBehavior.Interaction.Equals(DoorBehaviorTriggerType.Button) && doorBehavior.Opened && doorBehavior.NoObstacleExist)
                        {
                            doorBehavior.CloseDoor();
                        }    
                    }
                }
                if (hit.collider.CompareTag("Key"))
                {
                    // Traverse up the hierarchy and check for the IDoorBehavior component
                    IDoorBehavior doorBehavior = null;
                    while (currentTransform != null && doorBehavior == null)
                    {
                        doorBehavior = currentTransform.GetComponent<IDoorBehavior>();
                        currentTransform = currentTransform.parent;
                    }
                    doorBehavior.TakeKey(hit.collider.gameObject);
                }
                
                if (hit.collider.CompareTag("obstacle"))
                {
                    // Traverse up the hierarchy and check for the IDoorBehavior component
                    IDoorBehavior doorBehavior = null;
                    while (currentTransform != null && doorBehavior == null)
                    {
                        doorBehavior = currentTransform.GetComponent<IDoorBehavior>();
                        currentTransform = currentTransform.parent;
                    }
                    doorBehavior.DestroyObstacle(hit.collider.gameObject);
                }
                
            }   
        }
    }
    
    
    // Method to open the door when the "Open" action is performed "E pressed"
    private void OnOpenAction(InputAction.CallbackContext context)
    {
        
        foreach (var door in _doors)
        {
            if (door.Interaction.Equals(DoorBehaviorTriggerType.Keyboard) ||
                door.Interaction.Equals(DoorBehaviorTriggerType.Key))
            {
                if (door.NextToDoor && !door.IsOpening && door.KeyPicked && door.NoObstacleExist && !door.Opened)
                {
                    door.OpenDoor();
                    break;
                }
            
                if (door.NextToDoor && !door.IsOpening && door.KeyPicked && door.NoObstacleExist && door.Opened)
                {
                    door.CloseDoor();
                    break;
                }    
            }    
        }
        
        
        
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

    
    //get signals for new doors in scenes
    public void OnDoorAdded(GameObject door)
    {
        InitializeDoorBehaviors(door);
    }

    //destroys all gaming doors in the scene
    public void Clear()
    {
        foreach (var gamingDoor in _doors)
        {
            gamingDoor.deleteGameObject();
        }
            
        _doors.Clear();
    }

    private void OnDestroy()
    {
        // Unsubscribe when the object is destroyed
        FindObjectOfType<DoorCustomizationUI>()?.Unsubscribe(this);
    }
}
