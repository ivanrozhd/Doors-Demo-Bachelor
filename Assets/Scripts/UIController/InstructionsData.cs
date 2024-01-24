using UnityEngine;

[CreateAssetMenu(fileName = "InstructionsData", menuName = "Instructions/InstructionsData")]
public class InstructionsData : ScriptableObject
{
    public string DoorInstructions;
    public string ObstaclesInstructions;
    public string KeyInstructions;
    public string ButtonInstructions;
}