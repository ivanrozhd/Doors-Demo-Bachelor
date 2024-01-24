using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour8 : MonoBehaviour, IDoorBehavior
{
    [SerializeField] private float _openingTime = 5f;
    [SerializeField] private Transform _doorTransform;
    [SerializeField] private GameObject[] _obstacles;
    private bool _isOpening = false;
    private bool _opened = false;
    private bool _nextToDoor = false;
    private bool _noObstaclesExist = false;
    private TextDisplay _textDisplay;
    
    
    private void Start()
    {
        _textDisplay = GetComponent<TextDisplay>();
    }
    public void deleteGameObject()
    {
        Destroy(this.gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the trigger area
        if (other.CompareTag("Player"))
        {
            _nextToDoor = true;
            _textDisplay.ShowText(true);
         
        }
      
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player enters the trigger area
        if (other.CompareTag("Player"))
        {
            _nextToDoor = false;
            _textDisplay.ShowText(false);
        }
    }

    public DoorBehaviorTriggerType Interaction { get; set;}

    public DoorBehaviorType Type { get; set; }

    public void OpenDoor()
    {
        StartCoroutine(OpenAnimatedDoor());
    }

    public IEnumerator OpenAnimatedDoor()
    {
        float initialRotation = _doorTransform.rotation.eulerAngles.y;
        float targetRotation = initialRotation + 90f; // You can adjust the angle here

        float elapsedTime = 0f;

        _isOpening = true;

        while (elapsedTime < _openingTime)
        {
            float currentRotation = Mathf.Lerp(initialRotation, targetRotation, elapsedTime / _openingTime);
            _doorTransform.rotation = Quaternion.Euler(-90f, currentRotation, 0f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the door is fully open
        _doorTransform.rotation = Quaternion.Euler(-90f, targetRotation, 0f);
        _isOpening = false;
        _opened = true;
    }
    
    
    public void CloseDoor()
    {
        StartCoroutine(CloseAnimatedDoor());
    }


    public IEnumerator CloseAnimatedDoor()
    {
        float initialRotation = _doorTransform.rotation.eulerAngles.y;
        float targetRotation = 90f; // You can adjust the angle here

        float elapsedTime = 0f;

        _isOpening = true;

        while (elapsedTime < _openingTime)
        {
            float currentRotation = Mathf.Lerp(initialRotation, targetRotation, elapsedTime / _openingTime);
            _doorTransform.rotation = Quaternion.Euler(-90f, currentRotation, 0f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the door is fully open
        _doorTransform.rotation = Quaternion.Euler(-90f, targetRotation, 0f);
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
                Destroy(obstacle);
            }
        }
        
        if (_obstacles.Length == 0)
        {
            _noObstaclesExist = true;
            Debug.Log(_obstacles.Length );
        }
    }
    
    public void TakeKey(GameObject check)
    {
        return;
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
        get { return _noObstaclesExist; }
        set => _noObstaclesExist = value;
    }

    public bool KeyPicked
    
    {   
        get { return true; }
    }
    
    
}
