using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


// Script for the user interface to generate the unique door behaviours
public class DoorCustomizationUI : MonoBehaviour
{
    // Door manager is a player, no more players are presented in the scene
    private IDoorObserver _observer = null;

    public void Subscribe(IDoorObserver newObserver) 
    {
        _observer = newObserver;
    }

    public void Unsubscribe(IDoorObserver oldObserver) 
    {
        if (_observer == oldObserver)
        {
            _observer = null;
        }
    }
    
    // Essential UI elements of the door customizer
    public TMP_Dropdown FormOfDoor;
    public TMP_Dropdown TypeOfDoor;
    public TMP_Dropdown InteractionType;
    public Toggle obstaclesToggle;
    public TMP_Text Number;
    
    
    // Door Prefabs - for main door types
    [SerializeField] private GameObject _standardDoor;
    [SerializeField] private GameObject _thresholdDoor;
    [SerializeField] private GameObject _slidingDoor;
    [SerializeField] private GameObject _hingedDoor;

    
    // class variables 
    private GameObject _instantiatedDoor;
    private int form = -1;
    private int type;
    private Vector3 _offset = Vector3.forward;
    private int _numberOfDoors = 0; // the high boundary of the instansiated doors
    
    //Options adjustments to prevent the user from using not supported doors
    private List<TMP_Dropdown.OptionData> _typeOptions = new List<TMP_Dropdown.OptionData>();
    private List<TMP_Dropdown.OptionData> _interactionOptions = new List<TMP_Dropdown.OptionData>();
    
    
    // Called when the UI button for door creation is clicked
    public void CreateDoor()
    {
        if (_numberOfDoors == 30)
        {
            return;
        }
        
        switch (FormOfDoor.value)
        {
            case 0: _instantiatedDoor = Instantiate(_thresholdDoor, Vector3.zero + _offset, Quaternion.identity);
                break;
            case 1: _instantiatedDoor = Instantiate(_slidingDoor, Vector3.zero  + _offset, Quaternion.identity);
                break;
            case 2: _instantiatedDoor = Instantiate(_standardDoor, Vector3.zero  + _offset, Quaternion.identity);
                break;
            case 3: _instantiatedDoor = Instantiate(_hingedDoor, Vector3.zero  + _offset, Quaternion.Euler(0,90,0));
                break;
            default: Debug.LogError("No such door!");
                break;
        }
         
        // to prepare the scene for future instantiations 
        _offset += 5 * Vector3.forward;
        _numberOfDoors++;
        
        // Customize the door based on UI settings
        CustomizeDoor();
        
        // Notify the observer about new doors to define the connection between the player and gaming doors
        _observer.OnDoorAdded(_instantiatedDoor);
       
    }

    // update method checks the UI fields and extracts impossible combinations for the gaming doors
    private void Update()
    {
        if (form != FormOfDoor.value)
        {   
            // create the suitable options based on the form of the door
            TypeOfDoor.ClearOptions();
            InteractionType.ClearOptions();
            _interactionOptions.Clear();
            _typeOptions.Clear();
            
            
            switch (FormOfDoor.value)
            {
            
                case 0:  
                    _typeOptions.Add(new TMP_Dropdown.OptionData("None"));
                    
                    _interactionOptions.Add(new TMP_Dropdown.OptionData("None"));
                    break;
                case 1: 
                    _typeOptions.Add(new TMP_Dropdown.OptionData("Animated"));
                    _typeOptions.Add(new TMP_Dropdown.OptionData("TwoState"));
                    _typeOptions.Add(new TMP_Dropdown.OptionData("Distance"));
                    
                    _interactionOptions.Add(new TMP_Dropdown.OptionData("Trigger Opening"));
                    _interactionOptions.Add(new TMP_Dropdown.OptionData("Key"));
                    _interactionOptions.Add(new TMP_Dropdown.OptionData("Keyboard button"));
                    _interactionOptions.Add(new TMP_Dropdown.OptionData("Button"));
                    break;
                case 2: 
                    _typeOptions.Add(new TMP_Dropdown.OptionData("Animated"));
                    _typeOptions.Add(new TMP_Dropdown.OptionData("TwoState"));
                    _typeOptions.Add(new TMP_Dropdown.OptionData("Distance"));
                    
                    _interactionOptions.Add(new TMP_Dropdown.OptionData("Trigger Opening"));
                    _interactionOptions.Add(new TMP_Dropdown.OptionData("Key"));
                    _interactionOptions.Add(new TMP_Dropdown.OptionData("Keyboard button"));
                    _interactionOptions.Add(new TMP_Dropdown.OptionData("Button"));
                    break;
                case 3: 
                    _typeOptions.Add(new TMP_Dropdown.OptionData("180 degrees"));
                    _typeOptions.Add(new TMP_Dropdown.OptionData("Mouse clicking"));
                    
                    _interactionOptions.Add(new TMP_Dropdown.OptionData("None"));
                    _interactionOptions.Add(new TMP_Dropdown.OptionData("Key"));
                    break;
                default: break;
                
            }
            
            TypeOfDoor.AddOptions(_typeOptions);
            InteractionType.AddOptions(_interactionOptions);
            form = FormOfDoor.value;
        }
        
        if ((form == 2 || form == 1) && TypeOfDoor.value != type)
            
        {
            if (TypeOfDoor.value == 2)
            {
                _interactionOptions.Clear();
                InteractionType.ClearOptions();
                _interactionOptions.Add(new TMP_Dropdown.OptionData("None"));
                InteractionType.AddOptions(_interactionOptions);
                obstaclesToggle.isOn = false;     
            }

            else
            {
                _interactionOptions.Clear();
                InteractionType.ClearOptions();
                _interactionOptions.Add(new TMP_Dropdown.OptionData("Trigger Opening"));
                _interactionOptions.Add(new TMP_Dropdown.OptionData("Key"));
                _interactionOptions.Add(new TMP_Dropdown.OptionData("Keyboard button"));
                _interactionOptions.Add(new TMP_Dropdown.OptionData("Button"));
                InteractionType.AddOptions(_interactionOptions);
            }

            type = TypeOfDoor.value;
        }

        if (TypeOfDoor.value == 2)
        {
            obstaclesToggle.isOn = false; 
        }
        
    }

    
    // start over method
    public void ClearScene()
    {
        _numberOfDoors = 0;
        Number.text = "Doors: " + _numberOfDoors.ToString();
        _offset = Vector3.forward;
        _observer.Clear();
        
    }

    
    // based on the door characteristics from UI, we prepare the new door in the scene based on characteristics, which we define for the new door
    public void CustomizeDoor()
    {
        // update the parameters regarding the gaming door
        Number.text = "Doors: " +  _numberOfDoors.ToString();
        form = FormOfDoor.value; 
        type = TypeOfDoor.value;
        int interaction = InteractionType.value;
        bool hasObstacles = obstaclesToggle.isOn;
        IDoorBehavior child = _instantiatedDoor.GetComponent<IDoorBehavior>();
        
        child.NoObstacleExist = !hasObstacles;

        if (form == 3)
        {
            switch (type)
            {
                case 0: child.Type = DoorBehaviorType.FullOpening;
                    break;
                case 1: child.Type = DoorBehaviorType.MouseOpening;
                    break;
                default: Debug.LogError("No such type!"); break;
            }
            switch (interaction)
            {
                case 0:
                    child.Interaction = DoorBehaviorTriggerType.None;
                    break;
                case 1: child.Interaction = DoorBehaviorTriggerType.Key;
                    break;
                default: Debug.LogError("No such interaction!"); break;
            }
        }
        if (form == 2 || form == 1)
        {
            
                switch (type)
                {
                    case 0: child.Type = DoorBehaviorType.Animated;
                        break;
                    case 1: child.Type = DoorBehaviorType.TwoState;
                        break;
                    case 2: child.Type = DoorBehaviorType.Distance;
                        break;
                    default: Debug.LogError("No such type!"); break;
                        
                }

                switch (interaction)
                {
                    case 0:
                        child.Interaction = DoorBehaviorTriggerType.Area;
                        break;
                    case 1: child.Interaction = DoorBehaviorTriggerType.Key;
                        break;
                    case 2: child.Interaction = DoorBehaviorTriggerType.Keyboard;
                        break;
                    case 3: child.Interaction = DoorBehaviorTriggerType.Button;
                        break;
                    default: Debug.LogError("No such interaction!"); break;
                }
                
        }
    }
   
}
