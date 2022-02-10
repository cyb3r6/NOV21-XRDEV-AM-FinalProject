using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonInteractable : MonoBehaviour
{
    [Serializable]
    public class ButtonPressedEvent: UnityEvent { }
    [Serializable]
    public class ButtonReleasedEvent : UnityEvent { }

    public ButtonPressedEvent OnButtonPressed;
    public ButtonReleasedEvent OnButtonReleased;

    public AudioClip buttonPressedClip;
    public AudioClip buttonReleasedClip;

    public Vector3 axis = new Vector3(0, -1, 0);
    public float maxDistance;
    public float returnSpeed = 10.0f;

    private Vector3 startingPosition;
    private Rigidbody rigidbody;
    private Collider collider;
    private bool isPressed = false;

    
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponentInChildren<Collider>();
        startingPosition = transform.position;
    }

    
    void FixedUpdate()
    {
        Vector3 worldAxis = transform.TransformDirection(axis);
        Vector3 endPoint = transform.position + worldAxis * maxDistance;

        float currentDistance = (transform.position - startingPosition).magnitude;
        RaycastHit info;

        float move = 0.0f;

        if(rigidbody.SweepTest(-worldAxis, out info, returnSpeed * Time.deltaTime + 0.005f))
        {
            // if we're hitting something, if the contact is < mean we are pressed, we will move downward
            move = (returnSpeed * Time.deltaTime) - info.distance;
        }
        else
        {
            move -= returnSpeed * Time.deltaTime;
        }

        float newDistance = Mathf.Clamp(currentDistance + move, 0, maxDistance);

        rigidbody.position = startingPosition + worldAxis * newDistance;

        if(!isPressed && Mathf.Approximately(newDistance, maxDistance))
        {
            isPressed = true;
            Debug.Log("button pressed");
            OnButtonPressed.Invoke();
        }
        else if(isPressed && !Mathf.Approximately(newDistance, maxDistance))
        {
            isPressed = false;

            OnButtonReleased.Invoke();
        }
    }
}
