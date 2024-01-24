using UnityEngine;


public enum DoorBehaviorType
{
    Animated,
    TwoState,
    Distance,
    None,
    FullOpening,
    MouseOpening
}


public enum DoorBehaviorTriggerType
{
    Area,
    Key,
    Button,
    Keyboard,
    None
}



public interface IDoorBehavior
{
    // used for animated and 2-state doors
    void OpenDoor();
    void CloseDoor();
    

    // doors which have some kind of obstacles
    public void DestroyObstacle(GameObject obstacle);

    // Key interaction with the precise door
    public void TakeKey(GameObject check);

    // destroys from the scene the object attached with this interface
    public void deleteGameObject();
    
    // Essential parameters to define the door - player relations
    public DoorBehaviorTriggerType Interaction { get; set; }
    
    public DoorBehaviorType Type
    { set; }
    
    bool IsOpening { get; }
    
    bool Opened { get; }
    
    bool NextToDoor { get; }
    public bool NoObstacleExist { set; get; }
    public bool KeyPicked { get; }
    
    
}

