using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour0: MonoBehaviour, IDoorBehavior
{
    [SerializeField] private Transform _doorTransform;
    private bool _isOpening = false;
    private bool _opened = false;
    private bool _nextToDoor = false;
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
        _isOpening = true;
        _doorTransform.rotation = Quaternion.Euler(-90f, 90f, 0f);
        _isOpening = false;
        _opened = true;
    }

    public void CloseDoor()
    {
        _isOpening = true;
        _doorTransform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        _isOpening = false;
        _opened = false;
      
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
        get { return true; }
        set => throw new System.NotImplementedException();
    }

    public bool KeyPicked
    
    {   
        get { return true; }
    }
    
    public void DestroyObstacle(GameObject obstacle)
    {
        return;
    }
}
