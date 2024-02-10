using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The class takes care of the thresholds and interactions with a player
public class ThresholdBehaviour : MonoBehaviour, IDoorBehavior
{
    
    private bool _nextToDoor = false;
    private bool _noObstaclesExist = false;
    
    
    // Obstacle GameObject;
    [SerializeField] private GameObject _obstacleObject;
    [SerializeField] private GameObject[] _obstacles;
   
    
    
    private void Start()
    {
        _obstacleObject.SetActive(!NoObstacleExist);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the trigger area
        if (other.CompareTag("Player"))
        {
            if (!_noObstaclesExist)
            {
                _nextToDoor = true;    
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player exists the trigger area
        if (other.CompareTag("Player"))
        {
            if (!_noObstaclesExist)
            {
                _nextToDoor = true;    
            }
            
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
        
        _noObstaclesExist = _obstacles.Length == 0;
    }
    
    public void deleteGameObject()
    {
        Destroy(this.gameObject);
    }
    
    public bool NextToDoor   
    {   
        get { return _nextToDoor; }
    }
    public bool NoObstacleExist

    {
        get { return _noObstaclesExist; }
        set => _noObstaclesExist = value;
    }
    
    
    // further methods are not relevant for the threshold and return default values
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
    public bool KeyPicked   
    {   
        get { return true; }
    }
    
   

}
