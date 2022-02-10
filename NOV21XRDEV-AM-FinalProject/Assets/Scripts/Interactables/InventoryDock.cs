using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InventoryDock : XRSocketInteractor
{
    private CanvasHelper canvasHelper;
    private Vector3 socketPositionScale = Vector3.one;
    private Vector3 originalInteractableLocalScale = Vector3.one;

    
    void Start()
    {
        canvasHelper = GetComponentInChildren<CanvasHelper>();

        hoverEntered.AddListener(canvasHelper.ScaleUp);
        hoverExited.AddListener(canvasHelper.ScaleDown);
    }

    protected override void OnSelectEntered(XRBaseInteractable interactable)
    {
        base.OnSelectEntered(interactable);

        if (IsSelecting(interactable))
        {
            float scaleToFit = interactable.colliders[0].bounds.GetScaleToFitInside(GetComponent<Collider>().bounds);

            socketPositionScale = transform.localScale * scaleToFit;

            originalInteractableLocalScale = interactable.transform.localScale;

            interactable.transform.localScale = socketPositionScale;
        }
        
    }

    protected override void OnSelectExited(XRBaseInteractable interactable)
    {
        base.OnSelectExited(interactable);

        interactable.transform.localScale = originalInteractableLocalScale;
    }
}
