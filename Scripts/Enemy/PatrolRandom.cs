using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class PatrolRandom: MonoBehaviour
{
    public Transform currentPatrolPoint;

    //list for patrol points assigned to this agent
    public Transform[] patrolPoints;
    //the current destination
    private int randomDestPoint;
    public NavMeshAgent agent;
    private Enemy enemy;
    private Animator anim;
    private bool isWaiting;

    private float waitTime;

    [SerializeField]
    private float minWaitTime;
    [SerializeField]
    private float maxWaitTime;

    // Start is called before the first frame update
    void Start()
    {
        //currentPatrolPoint = agent.destination.ToString();

        enemy = GetComponent<Enemy>();
        agent = GetComponentInParent<NavMeshAgent>();
        agent.autoBraking = false;
        randomDestPoint = Random.Range(0, patrolPoints.Length);
        anim = GetComponent<Animator>();

        GoToNextPoint();
    }

    void Update()
    {
        //choose next patrolPoint when the agent gets close to current one.
        if (!agent.pathPending && agent.remainingDistance < 0.5f && isWaiting == false && enemy.detectedEnemy == false)
        {
            isWaiting = true;
            agent.isStopped = true;
            StartCoroutine(WaitAtPatrolPoint());

        }

        if (!agent)
        {
            agent = GetComponentInParent<NavMeshAgent>();
        }

        if (enemy.detectedEnemy)
        {
            waitTime = 0;
        }
    }

    public void GoToNextPoint()
    {
        //returns null if no points
        if (patrolPoints.Length == 0)
        {
            return;
        }


        //send agent to current point
        agent.destination = patrolPoints[randomDestPoint].position;

        //choose next point in array
        //moving to start if needed
        randomDestPoint = (randomDestPoint + 1) % patrolPoints.Length;
        currentPatrolPoint = patrolPoints[randomDestPoint];
        //play patrol sound
    }

    IEnumerator WaitAtPatrolPoint()
    {
        waitTime = Random.Range(minWaitTime, maxWaitTime);
        //Debug.Log(waitTime);
        yield return new WaitForSeconds(waitTime);
        isWaiting = false;
        agent.isStopped = false;
        GoToNextPoint();
    }
}
