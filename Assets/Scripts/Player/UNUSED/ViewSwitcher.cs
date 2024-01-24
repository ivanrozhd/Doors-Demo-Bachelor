using UnityEngine;
using UnityEngine.InputSystem;

public class ViewSwitcher : MonoBehaviour
{
    private DefaultPlayerInput _defaultPlayerInput;
    private FirstPersonController _firstPersonController;
    private ThirdPersonController _thirdPersonController;
    private bool _isFirstPersonView = true;
    [SerializeField] private Vector3 _cameraOffset = new Vector3(0, 2, -3);
    private Transform _transform;
    private Transform _cameraTransform;
    private Vector3 _initialCameraPosition;

    private void Awake()
    {
        _firstPersonController = GetComponent<FirstPersonController>();
        _thirdPersonController = GetComponent<ThirdPersonController>();
        _defaultPlayerInput = new DefaultPlayerInput();
        _cameraTransform = Camera.main.transform;
        _initialCameraPosition = Camera.main.transform.localPosition;

    }
    
    
    private void OnEnable()
    { 
        
        _defaultPlayerInput.Switcher.CameraSwitcher.performed += SwitchView;
        _defaultPlayerInput.Switcher.CameraSwitcher.Enable();
    }

    private void OnDisable()
    {
        _defaultPlayerInput.Switcher.CameraSwitcher.performed -= SwitchView;
        _defaultPlayerInput.Switcher.CameraSwitcher.Disable();
    }

    
    
    private void SwitchView(InputAction.CallbackContext context)
    {
        _isFirstPersonView = !_isFirstPersonView;
        _firstPersonController.enabled = _isFirstPersonView;
        _thirdPersonController.enabled = !_isFirstPersonView;
        UpdateCameraPosition();
        
    }
    
    private void UpdateCameraPosition()
    {
        if (_isFirstPersonView)
        {
            _cameraTransform.localPosition = _initialCameraPosition;   
        }
        else
        {
            _cameraTransform.localPosition += _cameraOffset;    
            
        }
        
    }
    
    
}