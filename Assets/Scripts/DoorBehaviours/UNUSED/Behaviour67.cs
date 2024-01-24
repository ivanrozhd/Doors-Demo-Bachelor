using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour67 : MonoBehaviour, IDoorBehavior
{
    [SerializeField] private float _openingTime = 5f;
    [SerializeField] private Transform _doorTransform;
    [SerializeField] private GameObject _key;
    private bool _isOpening = false;
    private bool _opened = false;
    private bool _nextToDoor = false;
    private bool _keyPicked;
    private TextDisplay _textDisplay;
    
    
    private void Start()
    {
        _textDisplay = GetComponent<TextDisplay>();
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
    public void deleteGameObject()
    {
        Destroy(this.gameObject);
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
    
    

    public void TakeKey(GameObject check)
    {
        _keyPicked = true;
        Destroy(_key.gameObject);
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
    
    public bool KeyPicked
    
    {   
        get { return _keyPicked; }
    }
    
    public void DestroyObstacle(GameObject obstacle)
    {
        return;
    }
    
    public bool NoObstacleExist

    {
        get { return true; }
        set => throw new System.NotImplementedException();
    }
}
