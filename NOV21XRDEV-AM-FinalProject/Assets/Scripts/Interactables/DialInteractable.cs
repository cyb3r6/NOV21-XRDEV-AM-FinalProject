using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Rotate around a give axis in a limited range or snap it at integer steps
/// rotation can either be controller rotation (dial) or controller movement (lever)
/// </summary>
public class DialInteractable : XRBaseInteractable
{
    public enum InteractionType
    {
        Rotation,
        Pull
    }

    [Serializable]
    public class DialTurnedAngleEvent : UnityEvent<float> { }
    [Serializable]
    public class DialTurnedStepEvent : UnityEvent<int> { }

    [Serializable]
    public class DialChangedEvent : UnityEvent<DialInteractable> { }

    public InteractionType dialType = InteractionType.Rotation;
    public Rigidbody rotationRigidbody;
    public Vector3 localRotationAxis;
    public Vector3 localStartAxis;
    public float rotationMaximumAngle;

    [Tooltip("Steps the dial will change to")]
    public int steps = 0;
    public bool snapOnRelease = true;

    public float currentAngle;
    public int currentStep;

    public DialTurnedAngleEvent onDialAngleChanged;
    public DialTurnedStepEvent onDialStepChanged;
    public DialChangedEvent onDialChanged;

    private XRBaseInteractor grabbingInteractor;
    private Quaternion grabbedRotation;
    private Vector3 startingWorldAxis;
    private float stepSize;
    private Transform originalTransform;
    private Transform syncTransform;

    void Start()
    {
        localRotationAxis.Normalize();
        localStartAxis.Normalize();

        if(rotationRigidbody == null)
        {
            rotationRigidbody = GetComponentInChildren<Rigidbody>();
        }

        currentAngle = 0;

        GameObject obj = new GameObject("dial copy");
        originalTransform = obj.transform;
        originalTransform.SetParent(transform.parent);
        originalTransform.localRotation = transform.localRotation;
        originalTransform.localPosition = transform.localPosition;

        if(steps > 0)
        {
            stepSize = rotationMaximumAngle / steps;
        }
        else
        {
            stepSize = 0.0f;
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        var interactor = args.interactor;
        grabbedRotation = interactor.transform.rotation;
        grabbingInteractor = interactor;

        var syncObject = new GameObject("TemporaryDialSyncTransform");
        syncTransform = syncObject.transform;

        if(rotationRigidbody != null)
        {
            syncTransform.rotation = rotationRigidbody.transform.rotation;
            syncTransform.position = rotationRigidbody.transform.position;
        }
        else
        {
            syncTransform.rotation = transform.rotation;
            syncTransform.position = transform.position;
        }

        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if(snapOnRelease && steps > 0)
        {
            Vector3 right = transform.TransformDirection(localStartAxis);
            Vector3 up = transform.TransformDirection(localRotationAxis);

            float angle = Vector3.SignedAngle(startingWorldAxis, right, up);
            if(angle < 0)
            {
                angle = 360 + angle;
            }

            int step = Mathf.RoundToInt(angle / stepSize);
            angle = step * stepSize;

            if (angle != currentAngle)
            {
                onDialStepChanged.Invoke(step);
                onDialChanged.Invoke(this);
                currentStep = step;
            }

            Vector3 newRight = Quaternion.AngleAxis(angle, up) * startingWorldAxis;
            angle = Vector3.SignedAngle(right, newRight, up);

            currentAngle = angle;

            if(rotationRigidbody!= null)
            {
                Quaternion newRotation = Quaternion.AngleAxis(angle, up) * rotationRigidbody.rotation;
                rotationRigidbody.MoveRotation(newRotation);
            }
            else
            {
                Quaternion newRotation = Quaternion.AngleAxis(angle, up) * transform.rotation;
                transform.rotation = newRotation;
            }
        }

        Destroy(syncTransform.gameObject);
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if (isSelected)
        {
            if(updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed)
            {
                startingWorldAxis = originalTransform.TransformDirection(localStartAxis);

                Vector3 worldAxisStart = syncTransform.TransformDirection(localStartAxis);
                Vector3 worldRotationAxis = syncTransform.TransformDirection(localRotationAxis);

                float angle = 0.0f;
                Vector3 newRight = worldAxisStart;

                if(dialType == InteractionType.Rotation)
                {
                    Quaternion difference = grabbingInteractor.transform.rotation * Quaternion.Inverse(grabbedRotation);

                    newRight = difference * worldAxisStart;

                    // get the new angle between the original right and the new right along the up axis
                    angle = Vector3.SignedAngle(startingWorldAxis, newRight, worldRotationAxis);

                    if (angle < 0)
                    {
                        angle = 360 + angle;
                    }

                }
                else
                {
                    Vector3 centerToController = grabbingInteractor.transform.position - transform.position;
                    centerToController.Normalize();

                    angle = Vector3.SignedAngle(startingWorldAxis, newRight, worldRotationAxis);
                    if (angle < 0)
                    {
                        angle = 360 + angle;
                    }
                }

                if(angle > rotationMaximumAngle)
                {
                    float updiff = 360 - angle;
                    float lowerdiff = angle - rotationMaximumAngle;
                    if(updiff < lowerdiff)
                    {
                        angle = 0;
                    }
                    else
                    {
                        angle = rotationMaximumAngle;
                    }
                }

                float finalAngle = angle;

                if(!snapOnRelease && steps > 0)
                {
                    int step = Mathf.RoundToInt(angle / stepSize);
                    finalAngle = step * stepSize;

                    if(!Mathf.Approximately(finalAngle, currentStep))
                    {
                        onDialStepChanged.Invoke(step);
                        onDialChanged.Invoke(this);
                        currentStep = step;
                    }
                }

                newRight = Quaternion.AngleAxis(angle, worldRotationAxis) * startingWorldAxis;
                angle = Vector3.SignedAngle(worldAxisStart, newRight, worldRotationAxis);
                Quaternion newRotation = Quaternion.AngleAxis(angle, worldRotationAxis);

                newRight = Quaternion.AngleAxis(finalAngle, worldRotationAxis) * startingWorldAxis;
                currentAngle = finalAngle;
                onDialAngleChanged.Invoke(finalAngle);
                onDialChanged.Invoke(this);

                finalAngle = Vector3.SignedAngle(worldAxisStart, newRight, worldRotationAxis);
                Quaternion newRBRotation = Quaternion.AngleAxis(finalAngle, worldRotationAxis) * syncTransform.rotation;

                if(rotationRigidbody != null)
                {
                    rotationRigidbody.MoveRotation(newRBRotation);
                }
                else
                {
                    transform.rotation = newRBRotation;
                }

                syncTransform.rotation = newRotation;
                grabbedRotation = grabbingInteractor.transform.rotation;
            }
        }

    }
   

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Handles.color = new Color(1.0f, 0.0f, 0.0f, 0.2f);
        Handles.DrawSolidArc(transform.position, transform.TransformDirection(localRotationAxis), transform.TransformDirection(localStartAxis), rotationMaximumAngle, 0.2f);
    }

#endif

}
