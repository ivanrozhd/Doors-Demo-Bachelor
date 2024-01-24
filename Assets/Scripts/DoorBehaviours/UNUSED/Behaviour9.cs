using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Behaviour9 : MonoBehaviour, IDoorBehavior
{
    // Start is called before the first frame update
    private bool _nextToDoor = false;
    private TextDisplay _textDisplay;
    void Start()
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

    public DoorBehaviorTriggerType Interaction { get; set; }

    public DoorBehaviorType Type { get; set; }

    public void OpenDoor()
    {
        Debug.Log("HWHW");
        SceneManager.LoadScene(1);

    }

    public void CloseDoor()
    {
        return;
    }

    public bool IsOpening
    {
        get { return false; }
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
    
    public bool Opened
    {
        get { return false; }
    }


    public void DestroyObstacle(GameObject obstacle)
    {
        return;
    }

    public void TakeKey(GameObject check)
    {
        return;
    }
}
