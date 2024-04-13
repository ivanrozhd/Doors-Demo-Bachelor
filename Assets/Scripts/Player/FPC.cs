using UnityEngine;
using UnityEngine.InputSystem;

// First player controller: moving, looking and interaction with gaming doors, teleporting
public class FPC : MonoBehaviour
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

    public DoorManager _doorManager;
    

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
    }
    
    
    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
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
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            
            if (Physics.Raycast(ray, out hit))  
            {
                
                HandleInteraction(hit);  
            }
        }
    }
    
   

// Checking out what kind of the interaction we have
    private void HandleInteraction(RaycastHit hit)
    {
        switch (hit.collider.tag)
        {
            case "Interactable":
                DoorInteractionManager.Instance.HandleDoor(hit.collider.transform);
                break;
            case "Key":
                DoorInteractionManager.Instance.TakeKey(hit.collider.transform, hit.collider.gameObject);
                break;
            case "obstacle":
                DoorInteractionManager.Instance.DestroyObstacle(hit.collider.transform, hit.collider.gameObject);
                break;
        }
    }
  
    
    // Method to open the door when the "Open" action is performed "E pressed"
    private void OnOpenAction(InputAction.CallbackContext context)
    {
        foreach (var door in _doorManager._doors)
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

   
}
