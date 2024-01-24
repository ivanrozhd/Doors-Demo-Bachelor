using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThresholdBehaviour : MonoBehaviour, IDoorBehavior
{
    
    private bool _nextToDoor = false;
    private bool _NoObstaclesExist = false;
    
    
    // Obstacle GameObject;
    [SerializeField] private GameObject _obstacleObject;
    [SerializeField] private GameObject[] _obstacles;
    private TextInstructions _textDisplay;
    
    
    private void Start()
    {
        _textDisplay = GameObject.FindGameObjectWithTag("Instructions").GetComponent<TextInstructions>();
        _obstacleObject.SetActive(!NoObstacleExist);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the trigger area
        if (other.CompareTag("Player") && !_NoObstaclesExist)
        {
            _nextToDoor = true;
            _textDisplay.Show(true,1);
         
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player enters the trigger area
        if (other.CompareTag("Player") && !_NoObstaclesExist)
        {
            _nextToDoor = false;
            _textDisplay.Show(false, 1);
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
                Destroy(obstacle);
            }
        }
        
        if (_obstacles.Length == 0)
        {
            _NoObstaclesExist = true;
            _textDisplay.Show(false, 1);
        }
    }
    
    public void TakeKey(GameObject check)
    {
       return;
    }

    public DoorBehaviorTriggerType Interaction { get; set; }

    public DoorBehaviorType Type { get; set; }

    public void OpenDoor()
    {
        return;
    }

    public void CloseDoor()
    {
        return;
    }

    public bool IsOpening   
    {   
        get { return false; }
    }
    public bool Opened   
    {   
        get { return true; }
    }
    public bool NextToDoor   
    {   
        get { return _nextToDoor; }
    }
    public bool NoObstacleExist

    {
        get { return _NoObstaclesExist; }
        set => _NoObstaclesExist = value;
    }

    public bool KeyPicked   
    {   
        get { return true; }
    }
    
    public void deleteGameObject()
    {
        Destroy(this.gameObject);
    }

}
