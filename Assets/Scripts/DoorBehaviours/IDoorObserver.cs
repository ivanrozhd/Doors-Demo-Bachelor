using UnityEngine;

public interface IDoorObserver 
{
    void OnDoorAdded(GameObject door);
    
    void Clear();
    
}