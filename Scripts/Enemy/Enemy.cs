using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    public bool idleEnemy; // set in inspector for idle enemy at play

    public bool startIdleEnemy; // switch to change this player from idle to active
    [SerializeField]
    private bool startDisabled; // used for triggering enemy spawn

    public int enemyNumber; // used to identify what enemy
    private GameManager gm;
    private PlayerCaught playerCaught;
    private UIManager UImanager;
    private Animator anim;
    private PatrolRandom patrolRandom;
    private AudioManager aud;
    [SerializeField]
    private NavMeshAgent navMesh;
    [SerializeField]
    private EnemyVoiceManager enemyVoice;

    public enum EnemyAttackType { MELEE, RANGED }//this might not be needed - instead tied to enemyRange (so melee will be 0, ranged will be >0)
    public EnemyAttackType SetAttackType; // could be kept just for editor readability

    [SerializeField]private float enemyRange; // range of weapon they're using

    public enum InvestigationState { CHECKHIDINGSPOTS, INVESTIGATEEVIDENCE, SEARCHAREA, PLACETRAP }
    public InvestigationState CurrentInvestigationState;

    public enum EnemyState { PATROLLING, INVESTIGATING, PURSUING, CONVERSING, TRAPPED }// SEARCHAREA, PLACETRAP, INVESTIGATEEVIDENCE, 
    public EnemyState CurrentEnemyState;

    public enum TrappedState { INJURED, UNCONSIOUS, IMMOBILIZED }// SEARCHAREA, PLACETRAP, INVESTIGATEEVIDENCE, 
    public TrappedState CurrentTrappedState;

    public GameObject player;
    public GameObject InvestigateLocation;
    public GameObject BearTrap; // TODO - this should be a container for applicable traps for this enemy (eg. grandfather lay beartrap, twigtrap, possibly nettrap)
    public GameObject EnemyHolder; // for the container this whole enemy is stored in. Used in StartDisabled Coroutine

    [SerializeField]
    private GameObject headTarget;
    [SerializeField]
    private GameObject defaultHeadTarget;
    [SerializeField]
    private Vector3 headTargetPosition;

    public LayerMask viewMask;

    public Vector3 lastPlayerPosition;
    public Transform playerPosition;
    public Transform[] spawnPoints;
    public Transform InvestigateLocationPosition; // create a transform of the position of noise/evidence etc
    [SerializeField]
    private Transform conversingSpawnPoint; //stores the position the enemy was at during runtime. for returning once sound is investigated

    public Light spotlight;

    Color originalSpotlightColour;

    public float destinationReachedTreshold = 3;
    public float distanceToTarget;
    public float viewDistance = 25;
    public float viewAngle = 180;
    public float patrolSpeed;
    public float attackSpeed;
    public float investigationWaitTime;
    [SerializeField]
    private float searchRadius;
    private float playerVisibleTimer;
    public float timeToSpotPlayer = 0.5f;

    private int lateStart = 2;
    private int spawnPosition;

    public bool detectedEnemy;
    public bool searchingForPlayer;
    public bool movingToEvidence;
    public bool investigatingEvidence;

    private bool hasStartedConversing;

    private void Awake()
    {
        CurrentInvestigationState = default;
    }

    private void Start()
    {
        UImanager = FindObjectOfType<UIManager>();
        anim = GetComponentInChildren<Animator>();
        patrolRandom = GetComponent<PatrolRandom>();
        gm = FindObjectOfType<GameManager>();
        playerCaught = FindObjectOfType<PlayerCaught>();
        aud = FindObjectOfType<AudioManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        navMesh = GetComponentInParent<NavMeshAgent>();
        playerPosition = player.transform;
        headTargetPosition =  defaultHeadTarget.transform.localPosition;
        //Debug.Log(headTargetPosition);

        searchRadius = 100f;
        //StartCoroutine(LateStart(lateStart)); //disable this for now
        if (startDisabled)
        {
            StartCoroutine("StartDisabled");
        }

        viewAngle = spotlight.spotAngle;
        originalSpotlightColour = spotlight.color;

        if(!idleEnemy)
        {
            return;
        } // storing conversing transform to return to
        else
        {
            conversingSpawnPoint = transform.parent;
            //Debug.Log("conversingSpawnPoint stored");
        }

        if (gm.DEBUGMODE)
        {
            spotlight.intensity = 4;
        } // spotlight for debug mode
    }

    //spawn enemy only after room generation has finished
    IEnumerator LateStart(float waitTime)
    {
        spawnPosition = Random.Range(0, spawnPoints.Length);
        yield return new WaitForSeconds(waitTime);
        Debug.Log("Late Start");
        transform.parent.position = spawnPoints[spawnPosition].position;
        transform.parent.rotation = spawnPoints[spawnPosition].rotation;
        transform.position = spawnPoints[spawnPosition].position;
    }

    IEnumerator StartDisabled()
    {

        yield return new WaitForSeconds(0.5f);
        EnemyHolder.SetActive(false);
        conversingSpawnPoint = transform;
    }


        private void FixedUpdate()
    {

        if (gm.DEBUGMODE)
        {
            spotlight.intensity = 4;
        }
        else
        {
            spotlight.intensity = 0;
        }

        anim.SetFloat("Moving", GetComponentInParent<NavMeshAgent>().velocity.magnitude);

        if (idleEnemy)
        {
            CurrentEnemyState = EnemyState.CONVERSING;

            patrolRandom.agent.isStopped = true;
            patrolRandom.enabled = false;
            anim.SetBool("isMoving", false);

            if (enemyVoice.conversingDialogue && hasStartedConversing == false)
            {
                hasStartedConversing = true;
                StartCoroutine("ConversationTimer"); 
            }

            if (startIdleEnemy)
            {
                StartIdleEnemy();
            }

            if (detectedEnemy || investigatingEvidence)
            {
                StartIdleEnemy();
                enemyVoice.aud.Stop();
                enemyVoice.pursuing = true;
                hasStartedConversing = false;
            }
        }

        if (CanSeePlayer())
        {
            playerVisibleTimer += Time.deltaTime;
            investigatingEvidence = false;
            headTarget.transform.DOMove(playerPosition.position, 0.5f);
            if (playerVisibleTimer > timeToSpotPlayer / 2) 
            {
                Debug.Log("Enemy thinks they see player");
                patrolRandom.agent.destination = playerPosition.position;
            }
        }
        else{
            if(CurrentEnemyState!= EnemyState.CONVERSING)
            {
                CurrentEnemyState = EnemyState.PATROLLING;
            }
            detectedEnemy = false;
            //searchingForPlayer = false;
            headTarget.transform.DOMove(defaultHeadTarget.transform.position, 0.5f);
            playerVisibleTimer -= Time.deltaTime;
            patrolRandom.agent.isStopped = false;
            patrolRandom.agent.speed = patrolSpeed; //Commented this to stop beartrap trigger from reverting speed
            if (patrolRandom == null) // check for patrolRandom script
            {
                patrolRandom = GetComponent<PatrolRandom>();
                if (patrolRandom.isActiveAndEnabled)
                {
                    patrolRandom.agent.speed = patrolSpeed;
                }
                else { return; }
            }
        }


        if (InvestigateLocation == player && !detectedEnemy)
        {
            Debug.Log("Ranged enemy moving to player");

            InvestigateLocationPosition = InvestigateLocation.transform; // any noise that enemy is within radius of will become "InvestigateLocation"
                                                                         //patrolRandom.agent.destination = InvestigateLocationPosition.transform.position; // next new patrol point becomes "InvestigateLocationPosition"

            distanceToTarget = Vector3.Distance(patrolRandom.agent.transform.position, InvestigateLocationPosition.transform.position); // measure distance from enemy to investigatelocation

            if (distanceToTarget <= enemyRange)
            {
                playerCaught.enemyCapturedBy = gameObject;
                playerCaught.RangedCapture();
            }

        }


        if (investigatingEvidence && movingToEvidence == true && !detectedEnemy)
        {
            CurrentEnemyState = EnemyState.INVESTIGATING;

            patrolRandom.enabled = false;

            InvestigateLocationPosition = InvestigateLocation.transform; // any noise that enemy is within radius of will become "InvestigateLocation"
            patrolRandom.agent.destination = InvestigateLocationPosition.transform.position; // next new patrol point becomes "InvestigateLocationPosition"


            distanceToTarget = Vector3.Distance(patrolRandom.agent.transform.position, InvestigateLocationPosition.transform.position); // measure distance from enemy to investigatelocation
                                                                                                                                        // 
            if (distanceToTarget < destinationReachedTreshold)
            {
                movingToEvidence = false;
                Investigate();
            }
            else // enemy is still moving towards evidence
            {
                movingToEvidence = true;
            }

        }

        if (!detectedEnemy && !investigatingEvidence) // reverts enemy to default patrol 
        {
            if (CurrentEnemyState !=  EnemyState.CONVERSING)
            {
                patrolRandom.enabled = true;
            }
            else
            {
                patrolRandom.agent.destination = conversingSpawnPoint.transform.position; // if enemy has investigated sound during conversing and found nothing, return to conversingPointSpawn
            }

        }


        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);

        if (playerVisibleTimer >= timeToSpotPlayer) // && searchingForPlayer == false
        {
            Attack();
            investigatingEvidence = false;
            //need to store detectedEnemy by a set amount after playerVisibleTimer < timeToSpotPlayer
        }
        else{
            detectedEnemy = false;
            anim.SetBool("isRunning", false);
        }

        if (CurrentEnemyState == EnemyState.PURSUING && !detectedEnemy)
        {
            StartCoroutine("LostSight");
        }
    }

    public void Attack()
    {
        if (player.GetComponent<PlayerCaught>().Captured == false && !investigatingEvidence)
        {
            if (idleEnemy)
            {
                startIdleEnemy = true;
                enemyVoice.StopCoroutine("ConversingVoice");
                if (enemyVoice.aud.isPlaying)
                {
                    enemyVoice.aud.Stop();
                }
            }

            CurrentEnemyState = EnemyState.PURSUING;
            StopAllCoroutines();

            detectedEnemy = true;
            searchingForPlayer = true;

            patrolRandom.enabled = false; // stop patrolling
            patrolRandom.agent.speed = attackSpeed; // start running
            //InvestigateLocation = player; // test for ranged enemy
            patrolRandom.agent.destination = player.transform.position; // set next waypoint to player
            lastPlayerPosition = playerPosition.transform.position;
            anim.SetBool("isLooking", false);
            anim.SetBool("isPlacing", false);
            anim.SetBool("isAngry", false);
            StartCoroutine("LostSight");
        }
    }

    public void Trapped()//starting point for player trapping enemies
    {
        CurrentEnemyState = EnemyState.TRAPPED;

        switch (CurrentTrappedState)
        {
            case TrappedState.INJURED:



                //be injured

                break;

            case TrappedState.UNCONSIOUS:

                StopAllCoroutines();
                //ragdoll enemy
                //stay in this state for x seconds
                //broadcast message
                //be unconsious

                break;

            case TrappedState.IMMOBILIZED:



                //be immobilized

                break;

        }
    }
    public void IdleEnemy()
    {
        idleEnemy = true;
        patrolRandom.agent.isStopped = true;
        //anim.SetBool("isMoving", false);
    }

    public bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, playerPosition.position) < viewDistance)
        {
            Vector3 dirToPlayer = (playerPosition.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position, playerPosition.position, viewMask) && player.activeInHierarchy == true && !gm.DEBUGMODE)
                {                    
                    return true;
                }
            }
        }
        return false;
    }

    public void Investigate()
    {
        CurrentEnemyState = EnemyState.INVESTIGATING;
        CurrentInvestigationState = (InvestigationState)Random.Range(0, System.Enum.GetValues(typeof(InvestigationState)).Length);
        Debug.Log(gameObject.transform.parent.name +  " investigation state is currently " + CurrentInvestigationState);

        switch (CurrentInvestigationState)
        {            
            case InvestigationState.CHECKHIDINGSPOTS:

                if (distanceToTarget < destinationReachedTreshold)
                {

                    //point head bone toward evidence for x seconds 
                    //Debug.Log("investigate evidence");
                    patrolRandom.agent.isStopped = true;
                    if (anim.GetBool("isLooking") == false)
                    {
                        anim.SetBool("isMoving" , false);
                        anim.SetBool("isLooking", true);
                    }

                    //ONCE AT PATROL POINT - CHECK HIDINGSPOT (OPEN CLOSET, LOOK UNDER BED)

                    StartCoroutine("InvestigateHidingSpotTimer");  //resume patrol after x seconds
                }
                break;


            case InvestigationState.INVESTIGATEEVIDENCE:

                if (distanceToTarget < destinationReachedTreshold)
                {
                    //point head bone toward evidence for x seconds 
                    patrolRandom.agent.isStopped = true;
                    if (anim.GetBool("isLooking") == false)
                    {
                        anim.SetBool("isLooking", true);
                    }
                    StartCoroutine("InvestigateTimer");  //resume patrol after x seconds
                }
                break;

            case InvestigationState.PLACETRAP:

                //Debug.Log("Moving to destination " + distanceToTarget);

                if (distanceToTarget < destinationReachedTreshold)
                {
                    patrolRandom.agent.isStopped = true;
                    if (anim.GetBool("isPlacing") == false)
                    {
                        anim.SetBool("isPlacing", true); // set this up in animator
                    }
                    StartCoroutine("PlaceTrapTimer");
                    //StartCoroutine("InvestigateTimer");  //resume patrol after x seconds
                }
                //IF the enemy has spotted player in past spawn random trap and place in area around evidence 
                //voice prompt
                break;

            case InvestigationState.SEARCHAREA:

                if (distanceToTarget < destinationReachedTreshold)
                {
                    patrolRandom.agent.isStopped = true;
                    if (anim.GetBool("isAngry") == false)
                    {
                        anim.SetBool("isAngry", true);
                    }
                    //InvestigateLocationPosition.transform.position = new Vector3(Random.Range(0f, 3f), 0f, 0f);//pick a random point within the radius of the evidence
                    //move to that point for x seconds
                    StartCoroutine("InvestigateTimer");  //resume patrol after x seconds
                }
                break;

            default:

                break;

        }
    }

    public void StartIdleEnemy()
    {
        idleEnemy = false;
        //anim.SetBool("isMoving", true);
        patrolRandom.agent.isStopped = false;
        patrolRandom.enabled = true;
    }

    public IEnumerator PlaceTrapTimer()
    {
        yield return new WaitForSeconds(2.5f);
        if (gm.trapPlaced)
        {
            anim.SetBool("isPlacing", false);
            //enemyVoice.PlayRandomPursuingVoice();
            patrolRandom.agent.isStopped = false;
            patrolRandom.enabled = true;
        }
        else
        {
            Instantiate(BearTrap, gameObject.transform.position + transform.forward * 1, Quaternion.identity);
            gm.trapPlaced = true;
            anim.SetBool("isPlacing", false);
            //enemyVoice.PlayRandomPursuingVoice();
            patrolRandom.agent.isStopped = false;
            patrolRandom.enabled = true;
        }
    }

    public IEnumerator InvestigateTimer()
    {
        investigatingEvidence = false;
        yield return new WaitForSeconds(investigationWaitTime);
        distanceToTarget = 4;
        anim.SetBool("isAngry",false);
        anim.SetBool("isLooking", false);
        anim.SetBool("isPlacing",false);
        //enemyVoice.PlayRandomPursuingVoice();
        patrolRandom.agent.isStopped = false;
        patrolRandom.agent.ResetPath();
        patrolRandom.enabled = true;
        searchingForPlayer = false;
    }

    public IEnumerator InvestigateHidingSpotTimer()
    {
        yield return new WaitForSeconds(investigationWaitTime);
        anim.SetBool("isLooking", false);
        patrolRandom.agent.isStopped = false;

        investigatingEvidence = false;

        GameObject scHolder = new GameObject(); // CREATE CONTAINER FOR COLLIDER
        scHolder.layer = 12; //ASSIGN IT TO ITS OWN LAYER - TO AVOID EXTERNAL TRIGGERS
        scHolder.transform.position = gameObject.transform.position; //MAKE ITS WORLD POSITION = ENEMY POSITION
        Vector3 scHolderPos = scHolder.transform.localPosition; // STORE THE CONTAINERS POSITION
        scHolder.name = "HidingSpotSphereCollider"; // NAME
        SphereCollider sc = scHolder.AddComponent(typeof(SphereCollider)) as SphereCollider; //CREATE COLLIDER 
        Physics.IgnoreLayerCollision(12,0); // DONT COLLIDE WITH ANYTHING ON THE DEFAULT LAYER
        int layerMask = 1 << 11; //Layer 11 - HIDINGSPOT

        sc.isTrigger = true; // SET AS TRIGGER
        sc.radius = searchRadius; //SET RADIUS OF TRIGGER - CAN BE ALTERED DEPENDING ON LEVEL

        Collider[] hidingSpotColliders = Physics.OverlapSphere(scHolderPos, sc.radius, layerMask);//GET COLLIDERS WITHIN TRIGGER RADIUS - IGNORE ANYTHING NOT HIDINGSPOT LAYER


        if (hidingSpotColliders.Length > 0) // CONTINUE ONLY IF THERE ARE *ANY* HIDING SPOTS IN THE SEARCHRADIUS
        {
            //int HidingSpots = Random.Range(0, hidingSpotColliders.Length); // ROLL FOR RANDOM SPOT IN THE ARRAY

            Transform HidingSpotToCheck;

                Collider bestTarget = null;
                float closestDistanceSqr = Mathf.Infinity;
                Vector3 currentPosition = transform.position;
                foreach (Collider hidingSpot in hidingSpotColliders)
                {
                    Vector3 directionToTarget = hidingSpot.transform.position - currentPosition;
                    float dSqrToTarget = directionToTarget.sqrMagnitude;
                    if (dSqrToTarget < closestDistanceSqr)
                    {
                        closestDistanceSqr = dSqrToTarget;
                        bestTarget = hidingSpot;
                    }
                }
                HidingSpotToCheck = bestTarget.transform;

            Debug.Log("Hiding spots in range " + hidingSpotColliders.Length); // COUNT AMOUNT OF HIDING SPOTS IN RANGE
            Debug.Log("Hiding spot to check " + HidingSpotToCheck.transform.parent.name); // PRINT NAME OF CHOSEN HIDING SPOT
            //enemyVoice.PlayRandomInvestigatingVoice();
            Destroy(scHolder); // DELETE COLLIDER AS IT'S DONE ITS JOB ^ FINDING A 'HIDINGSPOTTOCHECK' AS ABOVE
            patrolRandom.enabled = true; // START PATROLLING AGAIN
            patrolRandom.agent.destination = HidingSpotToCheck.transform.position; // SET NEXT PATROL POINT TO BE THE HIDING SPOT
            searchingForPlayer = true; // BOOLEAN FOR REMOVING PLAYER FROM HIDINGSPOT IF ENEMY COMES IN CONTACT WITH IT
        }
        else
        {
            Debug.Log("Hiding spots in range " + hidingSpotColliders.Length);
        }

        yield return new WaitForSeconds(investigationWaitTime);
        Destroy(scHolder);
        investigatingEvidence = false;
        distanceToTarget = Vector3.Distance(patrolRandom.agent.transform.position, InvestigateLocationPosition.transform.position);
        anim.SetBool("isLooking", false);
        anim.SetBool("isPlacing", false);
        patrolRandom.agent.isStopped = false;
        patrolRandom.enabled = true;
        searchingForPlayer = false;
    }

    public IEnumerator LostSight() // need to store the last seen position of player and investigate that
    {
        float searchTime = Random.Range(2f,6f);

        //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //sphere.GetComponent<MeshRenderer>().material.color = Color.red;
        //sphere.GetComponent<SphereCollider>().enabled = false;
        //Instantiate(sphere, lastPlayerPosition, Quaternion.identity);

        yield return new WaitForSeconds(1f);
        if (!detectedEnemy) // this*
        {
            Debug.Log("Lost Sight, checking last known position " + lastPlayerPosition);
            patrolRandom.agent.destination = lastPlayerPosition;        //move there and wait a set amount

            if (startIdleEnemy == false)
            {
                startIdleEnemy = true;
            }

            if (detectedEnemy == false) // *and this look weird together
            {
                GameObject lastKnownPositionSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                lastKnownPositionSphere.transform.position = lastPlayerPosition;
                lastKnownPositionSphere.GetComponent<MeshRenderer>().enabled = false;
                lastKnownPositionSphere.GetComponent<SphereCollider>().enabled = false;
                //investigatingEvidence = true;
                searchingForPlayer = true;
                //movingToEvidence = true;
                InvestigateLocation = lastKnownPositionSphere;
                Destroy(lastKnownPositionSphere);
            }

            yield return new WaitForSeconds(searchTime);
            searchingForPlayer = false;
            //movingToEvidence = false;
            if (detectedEnemy)
            {
                yield return null; //test for fixing enemy not reacting when waiting at lastPlayerPosition
            }
        }
    }

    public IEnumerator ConversationTimer()
    {
        if (hasStartedConversing)
        {
            Debug.Log("conversing");

            yield return new WaitForSeconds(enemyVoice.conversationLength);

            CurrentEnemyState = EnemyState.PATROLLING;
            StartIdleEnemy();
            patrolRandom.GoToNextPoint();
            hasStartedConversing = false;

        }

    }
}
