using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the class takes care of the sliding doors. Dynamic parts move apart in different direction.
public class SlidingDoorBehaviour : MonoBehaviour,IDoorBehavior
{
    // Combination parameters to define the specifics of the door interaction
    [SerializeField] private DoorBehaviorType _behaviorType;
    [SerializeField] private DoorBehaviorTriggerType _behaviorTrigger;

    // Dynamic part of the door used for the opening and closing
    [SerializeField] private Transform _doorTransformLeft;
    [SerializeField] private Transform _doorTransformRight;

    // Animation opening/closing
    [SerializeField] private float _openingTime = 3f;
    private Coroutine _openDoorCoroutine; 
    private Coroutine _closeDoorCoroutine; 


    // Must-have parameters to define more explicit whether the player can open the door/ interact with them
    // parameters are kind of signals for the door to perceive the environment around it
    private bool _isOpening = false;
    private bool _opened = false;
    private bool _nextToDoor = false;
    private bool _noObstaclesExist = false;
    private bool _keyPicked = false;

    
    // Distance 
    private float _activationDistance = 5f; // Distance at which the door starts opening
    private float _fullOpenDistance = 1f; // Distance at which the door is fully open
    private Vector3 _closedLeftPosition; 
    private Vector3 _closedRightPosition; 
    private Vector3 _openRightPosition; 
    private Vector3 _openLeftPosition; 
    private Transform _playerTransform;

    // Key attributes
    // signifies the right key from this concrete door
    [SerializeField] private GameObject _key;
    // presents the whole list of other keys in the scene. Needed to compare whether the player possess the right key from the door
    [SerializeField] private GameObject _keys;
    
    // Obstacle GameObjects - for activation in the scene
    [SerializeField] private GameObject _obstacleObject;

    // Button for door controll
    [SerializeField] private GameObject _button;

    // Obstacles
    [SerializeField] private GameObject[] _obstacles;
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        // Set the initial and target rotations of the door
        _closedRightPosition = _doorTransformRight.position;
        _closedLeftPosition = _doorTransformLeft.position;
        _openLeftPosition =  _doorTransformLeft.position -  new Vector3(1f, 0f, 0f);
        _openRightPosition = _doorTransformRight.position +  new Vector3(1f, 0f, 0f);
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        
        if (_behaviorType.Equals(DoorBehaviorType.Distance))
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            _behaviorTrigger = DoorBehaviorTriggerType.None;
        }

        else if (_behaviorType.Equals(DoorBehaviorType.Animated) || _behaviorType.Equals(DoorBehaviorType.TwoState))
        {
            _closedRightPosition = _doorTransformRight.localPosition;
            _closedLeftPosition = _doorTransformLeft.localPosition;
            _openLeftPosition =  _doorTransformLeft.localPosition -  new Vector3(1f, 0f, 0f);
            _openRightPosition = _doorTransformRight.localPosition +  new Vector3(1f, 0f, 0f);
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
          
            if (_behaviorTrigger.Equals(DoorBehaviorTriggerType.Key) && _keys != null)
            {
                _keys.SetActive(true);
            }

            if (_behaviorTrigger.Equals(DoorBehaviorTriggerType.Button) && _button != null)
            {
                _button.SetActive(true);
                _keyPicked = true;
                
            }
            
            if (_behaviorTrigger.Equals(DoorBehaviorTriggerType.Keyboard))
            {
                _keyPicked = true;
                
            }
         
        }
       
        if (_obstacles.Length == 0)
        {
            _noObstaclesExist = true;
        }
        
        _obstacleObject.SetActive(!_noObstaclesExist);
       
    }

    // Update method is primarily used for the Distance behaviour where we need to calculate the relative position between the player and door
    void Update()
    {
        if (_behaviorType.Equals(DoorBehaviorType.Distance))
        {
            float distanceToPlayer = Vector3.Distance(_doorTransformLeft.position, _playerTransform.position);

            if (distanceToPlayer < _activationDistance)
            {
                // Calculate openProgress based on player's distance
                float openProgress = Mathf.Clamp01((_activationDistance - distanceToPlayer) / (_activationDistance - _fullOpenDistance));

                // Calculate the new position for the left and right parts
                Vector3 newLeftPosition = Vector3.Lerp(_closedLeftPosition, _openLeftPosition, openProgress);
                Vector3 newRightPosition = Vector3.Lerp(_closedRightPosition, _openRightPosition, openProgress);

                // Update the positions of the left and right parts
                _doorTransformLeft.position = newLeftPosition;
                _doorTransformRight.position = newRightPosition;
            }
            else
            {
                // Player is out of range, ensure both parts are in their closed positions
                _doorTransformLeft.position = _closedLeftPosition;
                _doorTransformRight.position = _closedRightPosition;
            }
        }
    }

  
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the trigger area
        if (other.CompareTag("Player"))
        {
            _nextToDoor = true;
        }

    }
    
    
    void OnTriggerStay(Collider other)
    {
        // Code to run while another collider is within the trigger
        if (other.gameObject.CompareTag("Player") && _noObstaclesExist && !_opened)
        {
            if (_behaviorTrigger.Equals(DoorBehaviorTriggerType.Area))
            {
                OpenDoor();
                _opened = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player enters the trigger area
        if (other.CompareTag("Player") && _noObstaclesExist)
        {
            _nextToDoor = false;
            if (_behaviorTrigger.Equals(DoorBehaviorTriggerType.Area))
            {
                CloseDoor();
               
            }
        }
    }

    
    
    
    public void OpenDoor()
    {
        switch (_behaviorType)
        {
            case DoorBehaviorType.Animated:
                // Stop closing the door if it's currently closing
                if (_closeDoorCoroutine != null)
                {
                    StopCoroutine(_closeDoorCoroutine);
                    _closeDoorCoroutine = null;
                }
            
                // Start or resume opening the door
                if (_openDoorCoroutine == null)
                {
                    _openDoorCoroutine = StartCoroutine(OpenAnimatedDoor());
                }
                else
                {
                    // If the coroutine was paused, resume it
                    StopCoroutine(_openDoorCoroutine);
                    _openDoorCoroutine = StartCoroutine(OpenAnimatedDoor());
                }
                break;

            case DoorBehaviorType.TwoState:
                _isOpening = true;
                _doorTransformRight.localPosition = _openRightPosition;
                _doorTransformLeft.localPosition = _openLeftPosition;
                _isOpening = false;
                _opened = true;
                break;

            default:
                break;
        }
    }
    
    
    public IEnumerator OpenAnimatedDoor()
    {
        Vector3 initialPositionLeft = _doorTransformLeft.localPosition;
        Vector3 targetPositionLeft = _openLeftPosition;

        Vector3 initialPositionRight = _doorTransformRight.localPosition;
        Vector3 targetPositionRight = _openRightPosition;

        float elapsedTime = 0f;

        _isOpening = true;

        while (elapsedTime < _openingTime)
        {
            float t = elapsedTime / _openingTime;

            _doorTransformLeft.localPosition = Vector3.Lerp(initialPositionLeft, targetPositionLeft, t);
            _doorTransformRight.localPosition = Vector3.Lerp(initialPositionRight, targetPositionRight, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure both door parts are fully open
        _doorTransformLeft.localPosition = targetPositionLeft;
        _doorTransformRight.localPosition = targetPositionRight;
        _isOpening = false;
        _opened = true;
    }
    
    
    public IEnumerator CloseAnimatedDoor()
    {
        Vector3 initialPositionLeft = _doorTransformLeft.localPosition;
        Vector3 targetPositionLeft = _closedLeftPosition;

        Vector3 initialPositionRight = _doorTransformRight.localPosition;
        Vector3 targetPositionRight = _closedRightPosition;

        float elapsedTime = 0f;

        _isOpening = true;

        while (elapsedTime < _openingTime)
        {
            float t = elapsedTime / _openingTime;

            _doorTransformLeft.localPosition = Vector3.Lerp(initialPositionLeft, targetPositionLeft, t);
            _doorTransformRight.localPosition = Vector3.Lerp(initialPositionRight, targetPositionRight, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure both door parts are fully closed
        _doorTransformLeft.localPosition = targetPositionLeft;
        _doorTransformRight.localPosition = targetPositionRight;

        _isOpening = false;
        _opened = false;
    }
    
    
    

    public void CloseDoor()
    {
        switch (_behaviorType)
        {
            case DoorBehaviorType.Animated:
                if (_closeDoorCoroutine == null)
                {
                    _closeDoorCoroutine = StartCoroutine(CloseAnimatedDoor());
                }

                // Stop the door opening coroutine if it's running
                if (_openDoorCoroutine != null)
                {
                    StopCoroutine(_openDoorCoroutine);
                    _openDoorCoroutine = null;
                }
                break;

            case DoorBehaviorType.TwoState:
                _isOpening = true;
                _doorTransformRight.localPosition =  _closedRightPosition;
                _doorTransformLeft.localPosition = _closedLeftPosition;
                _isOpening = false;
                _opened = false;
                break;
            default:
                break;
        }
    }

  
    public void DestroyObstacle(GameObject obstacle)
    {
        foreach (var _obstacle in _obstacles)
        {
            if (_obstacle.Equals(obstacle))
            {
                List<GameObject> obstacleList = new List<GameObject>(_obstacles);
                obstacleList.Remove(obstacle);
                _obstacles = obstacleList.ToArray();
                _obstacle.SetActive(false);
            }
        }

        if (_obstacles.Length == 0)
        {
            _noObstaclesExist = true;
        }
    }

    public void TakeKey(GameObject check)
    {
        if (_behaviorTrigger.Equals(DoorBehaviorTriggerType.Key))
        {
            if (ReferenceEquals(check.gameObject, _key))
            {
                _keyPicked = true;   
            }
            Destroy(check.gameObject);
        }
    }
    
    
    public bool IsOpening
    {
        get { return _isOpening; }
    }
    
    public bool Opened
    {
        get { return _opened; }
    }

    public bool NextToDoor
    {
        get { return _nextToDoor; }
    }
    
    public bool NoObstacleExist

    {
        get { return this._noObstaclesExist; }
        set { this._noObstaclesExist = value; }
    }

    public bool KeyPicked
    
    {   
        get { return _keyPicked; }
    }

    public DoorBehaviorType Type
    {
        set { _behaviorType = value; }
    }

    public DoorBehaviorTriggerType Interaction
    {
        get { return _behaviorTrigger; }
        set { _behaviorTrigger = value; }
    }
    
    public void deleteGameObject()
    {
        Destroy(this.gameObject);
    }
    
}
