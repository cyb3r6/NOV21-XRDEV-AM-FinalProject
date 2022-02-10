using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public static class BoundsExtensions 
{
    /// <summary>
    /// Calculates how much scale is required for this Bounds to fit inside another bounds without stretching.
    /// </summary>
    /// <param name="containerBounds">The bounds of the container we're trying to fit this object.</param>
    /// <returns>A single scale factor that can be applied to this object to fit inside the container.</returns>
    public static float GetScaleToFitInside(this Bounds bounds, Bounds containerBounds)
    {
        var objectSize = bounds.size;
        var containerSize = containerBounds.size;
        Assert.IsTrue(objectSize.x != 0 && objectSize.y != 0 && objectSize.z != 0, "The bounds of the container must not be zero.");
        return Mathf.Min(containerSize.x / objectSize.x, containerSize.y / objectSize.y, containerSize.z / objectSize.z);
    }

   
}
