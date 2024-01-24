using UnityEngine;

public class Behaviour3 : MonoBehaviour
{
    [SerializeField] private Transform doorTransform;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float activationDistance = 10f; // Distance at which the door starts opening
    [SerializeField] private float fullOpenDistance = 2f; // Distance at which the door is fully open
    private Quaternion closedRotation; // Initial rotation of the door (closed state)
    private Quaternion openRotation; // Target rotation of the door (open state)

    private void Start()
    {
        // Set the initial and target rotations of the door
        closedRotation = doorTransform.rotation;
        openRotation = Quaternion.Euler(doorTransform.eulerAngles.x, doorTransform.eulerAngles.y, doorTransform.eulerAngles.z + 90); // Adjust the 90 degrees to your desired open angle
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(doorTransform.position, playerTransform.position);

        if (distanceToPlayer < activationDistance)
        {
            // Calculate openProgress based on player's distance
            float openProgress = Mathf.Clamp01((activationDistance - distanceToPlayer) / (activationDistance - fullOpenDistance));

            // Update the door's rotation based on the openProgress
            doorTransform.rotation = Quaternion.Lerp(closedRotation, openRotation, openProgress);
        }
        else
        {
            // Player is out of range, ensure the door is closed
            doorTransform.rotation = closedRotation;
        }
    }
}