using UnityEngine;
using TMPro;

public class TextInstructions : MonoBehaviour
{
    [SerializeField] private InstructionsData instructionsData;
    [SerializeField] private TextMeshProUGUI _doorInstructions;
    [SerializeField] private TextMeshProUGUI _obstaclesInstructions;
    [SerializeField] private TextMeshProUGUI _keyInstructions;
    [SerializeField] private TextMeshProUGUI _buttonInstructions;

    private void Start()
    {
        if (instructionsData != null)
        {
            _doorInstructions.text = instructionsData.DoorInstructions;
            _doorInstructions.enabled = false;
            _obstaclesInstructions.text = instructionsData.ObstaclesInstructions;
            _obstaclesInstructions.enabled = false;
            _keyInstructions.text = instructionsData.KeyInstructions;
            _keyInstructions.enabled = false;
            _buttonInstructions.text = instructionsData.ButtonInstructions;
            _buttonInstructions.enabled = false;
        }
    }

    public void Show(bool enable, int value)
    {
        switch (value)
        {
            case 0: _doorInstructions.enabled = enable;
                break;
            case 1: _obstaclesInstructions.enabled = enable;
                break;
            case 2: _keyInstructions.enabled = enable;
                break;
            case 3: _buttonInstructions.enabled = enable;
                break;
            default: Debug.LogError("No such data!");
                break;
        }
    }
}