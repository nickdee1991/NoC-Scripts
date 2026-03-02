using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseRangeEnable : MonoBehaviour
{
    [SerializeField]
    private ObjectiveParent objP;
    [SerializeField]
    private SphereCollider noiseRange;

    // Update is called once per frame
    void Update()
    {
        if (objP.hasCompletedObjective && noiseRange.enabled == false)
        {
            noiseRange.enabled = true;
        }
    }
}
