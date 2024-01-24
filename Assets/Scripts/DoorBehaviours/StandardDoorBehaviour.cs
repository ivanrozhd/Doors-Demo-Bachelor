using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StandardDoorBehaviour : MonoBehaviour, IDoorBehavior
{
    // Combination parameters
    [SerializeField] private DoorBehaviorType _behaviorType;
    [SerializeField] private DoorBehaviorTriggerType _behaviorTrigger;

    // Dynamic part of the door
    [SerializeField] private Transform _doorTransform;

    // Animation
    [SerializeField] private float _openingTime = 3f;
    private Coroutine _openDoorCoroutine; 
    private Coroutine _closeDoorCoroutine; 


    // Must-have parameters
    private bool _isOpening = false;
    private bool _opened = false;
    private bool _nextToDoor = false;
    private bool _noObstaclesExist = false;
    private bool _keyPicked = false;


    // Instructions
  //  private TextInstructions _textDisplay;


    // Distance 
    private float _activationDistance = 5f; // Distance at which the door starts opening
    private float _fullOpenDistance = 2f; // Distance at which the door is fully open
    private Quaternion _closedRotation; // Initial rotation of the door (closed state)
    private Quaternion _openRotation; // Target rotation of the door (open state)
    private Transform _playerTransform;

    // Key 
    [SerializeField] private GameObject _key;
    [SerializeField] private GameObject _keys;
    
    // Obstacle GameObject;
    [SerializeField] private GameObject _obstacleObject;

    // Button
    [SerializeField] private GameObject _button;

    // Obstacles
    [SerializeField] private GameObject[] _obstacles;


    private HingeJoint _hingeJoint;


    // Start is called before the first frame update
    void Start()
    {
        _obstacleObject.SetActive(!_noObstaclesExist);
        //_textDisplay = GameObject.FindGameObjectWithTag("Instructions").GetComponent<TextInstructions>();
        
        if (_behaviorType.Equals(DoorBehaviorType.Distance))
        {    _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            // Set the initial and target rotations of the door
            _closedRotation = _doorTransform.localRotation;
            _openRotation = Quaternion.Euler(_doorTransform.localRotation.eulerAngles.x, _doorTransform.localRotation.eulerAngles.y,
                _doorTransform.localRotation.eulerAngles.z + 90); // Adjust the 90 degrees to your desired open angle
            _behaviorTrigger = DoorBehaviorTriggerType.None;
        }

        else if (_behaviorType.Equals(DoorBehaviorType.Animated) || _behaviorType.Equals(DoorBehaviorType.TwoState))
        {
          
            if (_behaviorTrigger.Equals(DoorBehaviorTriggerType.Key) && _keys != null)
            {
                _keys.SetActive(true);
                //_textDisplay.Show(true, 2);
            }

            if (_behaviorTrigger.Equals(DoorBehaviorTriggerType.Button) && _button != null)
            {
                _button.SetActive(true);
                _keyPicked = true;
             //   _textDisplay.Show(true, 3);
                
            }
            
            if (_behaviorTrigger.Equals(DoorBehaviorTriggerType.Keyboard))
            {
                _keyPicked = true;
            }
            
            
        }
        
    }

    


    private void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the trigger area
        if (other.CompareTag("Player") && _noObstaclesExist)
        {
            _nextToDoor = true;
            
            if (_behaviorTrigger.Equals(DoorBehaviorTriggerType.Keyboard))
            {
               // _textDisplay.Show(true, 0);
            }
           
            if (_behaviorTrigger.Equals(DoorBehaviorTriggerType.Area))
            {
                OpenDoor();
            }
            
        }

        if (other.CompareTag("Player") && !_noObstaclesExist)
        {
           // _textDisplay.Show(true, 1);
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


    private void Update()
    {
        if (_behaviorType.Equals(DoorBehaviorType.Distance))
        {
            float distanceToPlayer = Vector3.Distance(_doorTransform.position, _playerTransform.position);

            if (distanceToPlayer < _activationDistance)
            {
                // Calculate openProgress based on player's distance
                float openProgress = Mathf.Clamp01((_activationDistance - distanceToPlayer) /
                                                   (_activationDistance - _fullOpenDistance));

                // Update the door's rotation based on the openProgress
                _doorTransform.localRotation = Quaternion.Lerp(_closedRotation, _openRotation, openProgress);
            }
            else
            {
                // Player is out of range, ensure the door is closed
                _doorTransform.localRotation = _closedRotation;
            }
        }

        if (_behaviorTrigger.Equals(DoorBehaviorTriggerType.Button) && _opened)
        {
          //  _textDisplay.Show(false, 3); 
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
                Debug.Log(_doorTransform.rotation.eulerAngles);
                _doorTransform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
                _isOpening = false;
                Debug.Log(_doorTransform.eulerAngles);
                _opened = true;
                break;
            default:
                Debug.LogError("No such opening!");
                break;
        }

    }

    public IEnumerator OpenAnimatedDoor()
    {
        float initialRotation = _doorTransform.localRotation.eulerAngles.y;
        float targetRotation = 0f; // angle for opening

        float elapsedTime = 0f;

        _isOpening = true;

        while (elapsedTime < _openingTime)
        {
            float currentRotation = Mathf.Lerp(initialRotation, targetRotation, elapsedTime / _openingTime);
            _doorTransform.localRotation = Quaternion.Euler(-90f, currentRotation, 0f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the door is fully open
        _doorTransform.localRotation = Quaternion.Euler(-90f, targetRotation, 0f);
        _isOpening = false;
        _opened = true;
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
                _doorTransform.localRotation = Quaternion.Euler(-90f, 90f, 0f);
                _isOpening = false;
                _opened = false;
                break;
            
            default: Debug.LogError("No such closing!");
                break;
        }
    }

    public IEnumerator CloseAnimatedDoor()
    {
        float initialRotation = _doorTransform.localRotation.eulerAngles.y;
        float targetRotation = 90f; // Angle for closing

        float elapsedTime = 0f;

        _isOpening = true;

        while (elapsedTime < _openingTime)
        {
            float currentRotation = Mathf.Lerp(initialRotation, targetRotation, elapsedTime / _openingTime);
            _doorTransform.localRotation = Quaternion.Euler(-90f, currentRotation, 0f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the door is fully closed
        _doorTransform.localRotation = Quaternion.Euler(-90f, targetRotation, 0f);
        _isOpening = false;
        _opened = false;
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
           // _textDisplay.Show(false, 1);
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
           // _textDisplay.Show(false, 2);
        }
    }

    public void deleteGameObject()
    {
        Destroy(this.gameObject);
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
}
