using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour, IDoorObserver
{
    //Extension of gaming doors to make sure that the player can interact with all of them
    public List<IDoorBehavior> _doors;
    private DoorCustomizationUI _doorCustomizationUI;


    private void Awake()
    {
        _doors = new List<IDoorBehavior>();
    }

    private void OnEnable()
    {
       _doorCustomizationUI = FindObjectOfType<DoorCustomizationUI>();
        if (_doorCustomizationUI != null)
        {
            _doorCustomizationUI.Subscribe(this);
        }
        else
        {
            Debug.LogWarning("DoorCustomizationUI not found in the scene.");
            
        }
    }

    private void OnDisable()
    {
        _doorCustomizationUI = FindObjectOfType<DoorCustomizationUI>();
        if (_doorCustomizationUI != null)
        {
            _doorCustomizationUI.Unsubscribe(this);
        }
    }

    public void OnDoorAdded(GameObject door)
    {
        InitializeDoorBehaviors(door);
    }

    public void Clear()
    {
        foreach (var gamingDoor in _doors)
        {
            gamingDoor.deleteGameObject();
        }
            
        _doors.Clear();
    }
    
    
    
    // check out when the new door is initialized in the scene
    public void InitializeDoorBehaviors(GameObject door)
    {
        IDoorBehavior doorBehavior = door.GetComponent<IDoorBehavior>();
        if (doorBehavior != null && !_doors.Contains(doorBehavior))
        {
            _doors.Add(doorBehavior); 
        }
    }
    
    
    private void OnDestroy()
    {
        // Unsubscribe when the object is destroyed
        FindObjectOfType<DoorCustomizationUI>()?.Unsubscribe(this);
    }
    
}
