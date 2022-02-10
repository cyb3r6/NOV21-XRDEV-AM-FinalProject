using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using DG.Tweening;

public class CanvasHelper : MonoBehaviour
{
    private Vector3 startScale;
    private float hoverEndAnimationDuration = 0.1f;
    private float hoverStartAnimationDuration = 0.2f;
    private float scaleAnimationSize = 0.2f;
    void Start()
    {
        startScale = transform.localScale;
    }

    public void ScaleUp(HoverEnterEventArgs args)
    {
        transform.DOScale(scaleAnimationSize, hoverStartAnimationDuration);
    }

    public void ScaleDown(HoverExitEventArgs args)
    {
        transform.DOScale(startScale, hoverEndAnimationDuration);
    }
}
