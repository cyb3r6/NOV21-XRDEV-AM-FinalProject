using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawnable : MonoBehaviour
{
    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private Rigidbody rigidbody;
   
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        if (rigidbody)
        {
            startingPosition = rigidbody.position;
            startingRotation = rigidbody.rotation;
        }
        else
        {
            startingPosition = transform.position;
            startingRotation = transform.rotation;
        }
    }

    public void Respawn()
    {
        if (rigidbody)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.position = startingPosition;
            rigidbody.rotation = startingRotation;
        }
        else
        {
            transform.position = startingPosition;
            transform.rotation = startingRotation;
        }
    }
   
}
