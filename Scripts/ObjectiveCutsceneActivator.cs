using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveCutsceneActivator : MonoBehaviour
{
    [SerializeField]
    private CutsceneTrigger cutsceneTrigger;

    [SerializeField]
    private Item objectiveChild;

    [SerializeField]
    private ObjectiveParent objectiveParent;

    public bool cutsceneTriggerActivated;

    private void Start()
    {
        objectiveChild = GetComponent<Item>();
        objectiveParent = GetComponent<ObjectiveParent>();
    }

    private void FixedUpdate()
    {
        if (objectiveChild != null)
        {
            if (objectiveChild.hasPickedUp)
            {
                cutsceneTrigger.cutsceneActivated = cutsceneTriggerActivated;
                cutsceneTriggerActivated = true;

            }
        }

        if (objectiveParent != null)
        {
            if (objectiveParent.hasCompletedObjective)
            {
                cutsceneTriggerActivated = true;
            }
        }
    }
}
