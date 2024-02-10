using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingedBehaviour : MonoBehaviour, IDoorBehavior
{
     // Combination parameters
    [SerializeField] private DoorBehaviorType _behaviorType;
    [SerializeField] private DoorBehaviorTriggerType _behaviorTrigger;
    
    // Control of the physical component
    [SerializeField] private HingeJoint _hingeJoint;
    [SerializeField] private Transform _doorTransform;
    private Quaternion initialRotation;
    private bool isDragging = false;
    private Vector3 mouseOffset;


    // Must-have parameters to define more explicit whether the player can open the door/ interact with them
    // parameters are kind of signals for the door to perceive the environment around it
    private bool _isOpening = false;
    private bool _opened = false;
    private bool _noObstaclesExist = false;
    private bool _keyPicked = false;
    private bool _nextToDoor = false;
    private JointMotor _hingeMotor;


    
    // Key attributes
    // signifies the right key from this concrete door
    [SerializeField] private GameObject _key;
    [SerializeField] private GameObject _keys;
    
    // Obstacle GameObjects - for activation in the scene
    [SerializeField] private GameObject _obstacleObject;
    [SerializeField] private GameObject _block;

    
    // Obstacles
    [SerializeField] private GameObject[] _obstacles;
    
    
    // Start is called before the first frame update
    void Start()
    {
        initialRotation = _doorTransform.localRotation;
        DraggableDoor _draggableDoorScript = _doorTransform.gameObject.GetComponent<DraggableDoor>();
        if (_behaviorType.Equals(DoorBehaviorType.FullOpening))
        {
            _hingeJoint.limits = new JointLimits
            {
                min = -90,
                max = 90 
                
            };
            if (_draggableDoorScript != null)
            {
                Destroy(_draggableDoorScript);
            }
            
        }
        if (_behaviorType.Equals(DoorBehaviorType.MouseOpening))
        {
            if (_draggableDoorScript != null)
            {
                _draggableDoorScript.enabled = true;
            }
        }
        
        if (_behaviorTrigger.Equals(DoorBehaviorTriggerType.Key) && _keys != null)
        {
                _keys.SetActive(true);
        }
            
        _obstacleObject.SetActive(!_noObstaclesExist);
    }

    // Make the door close after the player leaves the the door area
    void FixedUpdate()
    {
        if (_behaviorType.Equals(DoorBehaviorType.FullOpening))
        {
            if (!_nextToDoor)
            {
                
            }
            if (Quaternion.Angle(_doorTransform.localRotation, initialRotation) > 1f  && !_nextToDoor)
            {
                // Calculate the rotation speed based on the remaining angle to reach the initial position
                float remainingAngle = Quaternion.Angle(_doorTransform.localRotation, initialRotation);
                float rotationSpeed = Mathf.Lerp(1f, 3f, remainingAngle / 90f); // Adjust the values as needed
                // Interpolate the rotation to return the door to its initial position
                _doorTransform.localRotation = Quaternion.Slerp(_doorTransform.localRotation, initialRotation, rotationSpeed * Time.deltaTime);
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

    private void OnTriggerExit(Collider other)
    {
        // Check if the player enters the trigger area
        if (other.CompareTag("Player"))
        {
            _nextToDoor = false;
        }
    }

    

    public void OpenDoor() {return;}
   
    
    public void CloseDoor() {return;}
   

    
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

        _noObstaclesExist = _obstacles.Length == 0;

    }

    public void TakeKey(GameObject check)
    {
        if (_behaviorTrigger.Equals(DoorBehaviorTriggerType.Key))
        {
            
            if (ReferenceEquals(check.gameObject, _key))
            {
                _keyPicked = true;   
                _block.SetActive(false);
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
