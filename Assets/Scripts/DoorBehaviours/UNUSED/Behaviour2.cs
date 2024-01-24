using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Class demonstrates one potential interaction behaviour of the door
public class Behaviour2 : MonoBehaviour
{
    [SerializeField] private float _openingTime = 5f;
    [SerializeField] private Transform _doorTransform;
    private bool _isOpening = false;
    private Coroutine _openDoorCoroutine; 
    private Coroutine _closeDoorCoroutine; 
  
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the trigger area
        if (other.CompareTag("Player"))
        {
            // Stop closing the door if it's currently closing
            if (_closeDoorCoroutine != null)
            {
                StopCoroutine(_closeDoorCoroutine);
                _closeDoorCoroutine = null;
            }
            
            // Start or resume opening the door
            if (_openDoorCoroutine == null)
            {
                _openDoorCoroutine = StartCoroutine(OpenDoor());
            }
            else
            {
                // If the coroutine was paused, resume it
                StopCoroutine(_openDoorCoroutine);
                _openDoorCoroutine = StartCoroutine(OpenDoor());
            }

         
        }
      
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player exits the trigger area
        if (other.CompareTag("Player"))
        {
            // Start closing the door when the player exits the trigger area
            if (_closeDoorCoroutine == null)
            {
                _closeDoorCoroutine = StartCoroutine(CloseDoor());
            }

            // Stop the door opening coroutine if it's running
            if (_openDoorCoroutine != null)
            {
                StopCoroutine(_openDoorCoroutine);
                _openDoorCoroutine = null;
            }
        }
    }


    public IEnumerator OpenDoor()
    {
        float initialRotation = _doorTransform.rotation.eulerAngles.y;
        float targetRotation =  90f; // You can adjust the angle here

        float elapsedTime = 0f;

        _isOpening = true;

        while (elapsedTime < _openingTime)
        {
            float currentRotation = Mathf.Lerp(initialRotation, targetRotation, elapsedTime / _openingTime);
            _doorTransform.rotation = Quaternion.Euler(-90f, currentRotation, 0f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the door is fully open
        _doorTransform.rotation = Quaternion.Euler(-90f, targetRotation, 0f);
        _isOpening = false;
    }
    
    
    
    public IEnumerator CloseDoor()
    {
        float initialRotation = _doorTransform.rotation.eulerAngles.y;
        float targetRotation = 0f; // You can adjust the angle here

        float elapsedTime = 0f;

        _isOpening = true;

        while (elapsedTime < _openingTime)
        {
            float currentRotation = Mathf.Lerp(initialRotation, targetRotation, elapsedTime / _openingTime);
            _doorTransform.rotation = Quaternion.Euler(-90f, currentRotation, 0f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the door is fully open
        _doorTransform.rotation = Quaternion.Euler(-90f, targetRotation, 0f);
        _isOpening = false;
    }
    
}