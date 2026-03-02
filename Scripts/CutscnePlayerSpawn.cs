using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutscnePlayerSpawn : MonoBehaviour
{
    [SerializeField]
    private GameObject[] eventToSpawn;

    private GameObject selectedEvent;
    [SerializeField]
    private int eventRandomRoll;

    private CutsceneTrigger cutsceneTrigger;

    private void Start()
    {
        cutsceneTrigger = GetComponent<CutsceneTrigger>();

        eventRandomRoll = Random.Range(0, eventToSpawn.Length);
    }

    private void FixedUpdate()
    {
        if (cutsceneTrigger.cutscenePlaying)
        {
            //foreach (GameObject @event in eventToSpawn)
            //{
            selectedEvent = eventToSpawn[eventRandomRoll];
            selectedEvent.SetActive(true);
            //}

        }
    }

}
