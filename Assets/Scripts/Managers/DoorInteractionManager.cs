using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractionManager : MonoBehaviour
{
    public static DoorInteractionManager Instance { get; } = new DoorInteractionManager();

    public void HandleDoor(Transform transform)
    {
        IDoorBehavior doorBehavior = FindDoorBehavior(transform);
        if (doorBehavior != null && doorBehavior.NoObstacleExist)
        {
            if (doorBehavior.Interaction == DoorBehaviorTriggerType.Button)
            {
                if (!doorBehavior.Opened)
                    doorBehavior.OpenDoor();
                else
                    doorBehavior.CloseDoor();
            }
        }
    }

    public void TakeKey(Transform transform, GameObject key)
    {
        IDoorBehavior doorBehavior = FindDoorBehavior(transform);
        doorBehavior?.TakeKey(key);
    }

    public void DestroyObstacle(Transform transform, GameObject obstacle)
    {
        IDoorBehavior doorBehavior = FindDoorBehavior(transform);
        doorBehavior?.DestroyObstacle(obstacle);
    }

    private IDoorBehavior FindDoorBehavior(Transform transform)
    {
        while (transform != null)
        {
            var behavior = transform.GetComponent<IDoorBehavior>();
            if (behavior != null)
                return behavior;
            transform = transform.parent;
        }
        return null;
    }
}
