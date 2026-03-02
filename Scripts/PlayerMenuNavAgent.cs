using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMenuNavAgent : MonoBehaviour
{

    public Transform[] points;
    private int destPoint = 0;
    private NavMeshAgent agent;
    [SerializeField]
    private GameObject wheel1;
    [SerializeField]
    private GameObject wheel2;
    private float wheelRotation = 250;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = false;

        GotoNextPoint();
    }


    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
        Debug.Log("next point is "+ points[destPoint].position);
    }


    void FixedUpdate()
    {
        wheel1.transform.Rotate(new Vector3(0, 0, Time.deltaTime * wheelRotation));
        wheel2.transform.Rotate(new Vector3(0, 0, Time.deltaTime * wheelRotation));

        // Choose the next destination point when the agent gets
        // close to the current one.
        if (!agent.pathPending && agent.remainingDistance < 2f)
            GotoNextPoint();
    }
}
