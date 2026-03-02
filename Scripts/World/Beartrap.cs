using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beartrap : MonoBehaviour
{
    [SerializeField]
    private SphereCollider sphereCol;
    [SerializeField]
    GameObject teleportedEnemy;
    [SerializeField]
    Transform teleportedEnemyLastPatrolPoint;

    [SerializeField]
    public Transform enemySpawnPoint;

    private AudioSource aud;
    [SerializeField]
    private AudioClip[] bearTrapSound;

    [SerializeField]
    private Animator anim;

    private ParticleSystem blood;
    [SerializeField]
    private bool isTrapped;
    [SerializeField]
    private bool trappedCooldown;

    [SerializeField]
    private bool deployed; // for when player picks up trap, so interactable wont interfere wit inventory

    [SerializeField]
    private bool trapTriggered; // used for player to immobilize trap to proceed
    [SerializeField]
    private float trappedTime = 3f;
    private float trappedCooldownTime = 10f;
    [SerializeField]
    private float noiseRadius;

    [SerializeField]
    private Interactable interactable; // enabled once trap is triggered. to let player reset it

    private PlayerSimpleMovement playerSpeed;

    private void Start()
    {
        playerSpeed = FindObjectOfType<PlayerSimpleMovement>();
        aud = GetComponent<AudioSource>();
        blood = GetComponentInChildren<ParticleSystem>();
        anim = GetComponentInChildren<Animator>();
        //sphereCol = GetComponentInChildren<SphereCollider>();
        interactable = GetComponent<Interactable>();
        Physics.IgnoreCollision(gameObject.GetComponent<SphereCollider>(), GameObject.Find("NoiseRange").GetComponent<SphereCollider>(), true);
    }

    IEnumerator TeleportEnemyTimerSpawn()
    {
        //grab an enemy in the scene
        Enemy enemyToTeleport;
        int randNum;
        randNum = Random.Range(0, FindObjectOfType<GameManager>().allEnemiesInGame.Length);
        Debug.Log("enemy to spawn to beartrap " + randNum);
        enemyToTeleport = FindObjectOfType<GameManager>().allEnemiesInGame[randNum];
        teleportedEnemy = enemyToTeleport.gameObject.transform.parent.gameObject;
        Enemy teleportedEnemyEnemyScript = teleportedEnemy.GetComponent<Enemy>();
        Transform lastPatrolPoint;
        GetComponentInChildren<GetRandomPointInSphere>().GetRandomPointOnSpawn();
        //store teleported enemies last patrol point
        PatrolRandom teleportedEnemyPatrolRandom = teleportedEnemy.GetComponent<PatrolRandom>();
        lastPatrolPoint = teleportedEnemyPatrolRandom.currentPatrolPoint;

        //move them to a random spawn point
        //int destPoint = (Random.Range(0, enemySpawnPoint.Length));
        teleportedEnemy.transform.position = enemySpawnPoint.transform.position;

        //make the enemy next patrol point the bear trap
        if (teleportedEnemyEnemyScript.startIdleEnemy == false)
        {
            teleportedEnemyEnemyScript.startIdleEnemy = true;
        }

        if (teleportedEnemyEnemyScript.detectedEnemy == false)
        {
            teleportedEnemyEnemyScript.investigatingEvidence = true;
            teleportedEnemyEnemyScript.movingToEvidence = true;
            teleportedEnemyEnemyScript.InvestigateLocation = gameObject;
        }
        yield return new WaitForSeconds(2f);
    }

    IEnumerator TeleportEnemyTimer() //timer to check if enemy has not seen player and can return to its previous patrol point
    {
        //grab an enemy in the scene
        Enemy enemyToTeleport;
        int randNum;
        randNum = Random.Range(0, FindObjectOfType<GameManager>().allEnemiesInGame.Length);
        Debug.Log("enemy to spawn to beartrap " + randNum);
        enemyToTeleport = FindObjectOfType<GameManager>().allEnemiesInGame[randNum];
        teleportedEnemy = enemyToTeleport.gameObject;
        Enemy teleportedEnemyEnemyScript = teleportedEnemy.GetComponent<Enemy>();
        Transform lastPatrolPoint;



        //store teleported enemies last patrol point
        PatrolRandom teleportedEnemyPatrolRandom = teleportedEnemy.GetComponent<PatrolRandom>();
        lastPatrolPoint = teleportedEnemyPatrolRandom.currentPatrolPoint;

        teleportedEnemyLastPatrolPoint = lastPatrolPoint; // store patrol point

        //move them to a point within the sphere
        GetComponentInChildren<GetRandomPointInSphere>().GetRandomPointOnXAxis();
        GetComponentInChildren<GetRandomPointInSphere>().SetSpawnPoint();
        //teleport enemy to random sphere position
        teleportedEnemy.transform.position = GetComponentInChildren<GetRandomPointInSphere>().enemySpawnPoint;

        //make the enemy next patrol point the bear trap
        if (teleportedEnemyEnemyScript.startIdleEnemy == false)
        {
            teleportedEnemyEnemyScript.startIdleEnemy = true;
        }

        if (teleportedEnemyEnemyScript.detectedEnemy == false)
        {
            teleportedEnemyEnemyScript.investigatingEvidence = true;
            teleportedEnemyEnemyScript.movingToEvidence = true;
            teleportedEnemyEnemyScript.InvestigateLocation = gameObject;
        }

        yield return new WaitForSeconds(15f);

        StartCoroutine("TeleportedEnemyTimerRecheck");

    }

    public IEnumerator TeleportedEnemyTimerRecheck()
    {
        #region //storing enemy data again
        Enemy enemyToTeleport;
        int randNum;
        randNum = Random.Range(1, FindObjectOfType<GameManager>().allEnemiesInGame.Length);
        Debug.Log("enemy to spawn to beartrap " + randNum);
        enemyToTeleport = FindObjectOfType<GameManager>().allEnemiesInGame[randNum];
        teleportedEnemy = enemyToTeleport.transform.parent.gameObject;
        Enemy teleportedEnemyEnemyScript = teleportedEnemy.GetComponent<Enemy>();



        //store teleported enemies last patrol point
        PatrolRandom teleportedEnemyPatrolRandom = teleportedEnemy.GetComponent<PatrolRandom>();

        #endregion

        //after player has fled sphere
        if (GetComponent<GetRandomPointInSphere>().playerInRange)
        {
            //(as long as they aren't being pursued),
            if (teleportedEnemyEnemyScript.detectedEnemy == false)
            {
                //teleport enemy back to the previous patrol point they were heading to
                teleportedEnemyPatrolRandom.currentPatrolPoint = teleportedEnemyLastPatrolPoint;
                enemyToTeleport.gameObject.transform.position = teleportedEnemyPatrolRandom.currentPatrolPoint.position;
            }
            else
            {
                yield return new WaitForSeconds(15f);
                StartCoroutine("TeleportedEnemyTimerRecheck");
            }
        }
    }


    public void FixedUpdate()
    {
        if(gameObject.GetComponent<SphereCollider>() && GameObject.Find("NoiseRange").GetComponent<SphereCollider>() != null)
        {
            Physics.IgnoreCollision(gameObject.GetComponent<SphereCollider>(), GameObject.Find("NoiseRange").GetComponent<SphereCollider>(), true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<PlayerSimpleMovement>().isCreeping == false && !isTrapped && !trappedCooldown)
        {
            Debug.Log(collision.gameObject.name + "has activated trap");
            //StartCoroutine("PlayerTrapped");
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isTrapped)
        {
            isTrapped = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Physics.IgnoreCollision(gameObject.GetComponent<SphereCollider>(), GameObject.Find("NoiseRange").GetComponent<SphereCollider>(),true); // and probably hand colliders too

        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PlayerSimpleMovement>().isCreeping == false && !isTrapped && !trappedCooldown)
        {
            Debug.Log(other.gameObject.name + " has activated trap");
            StartCoroutine("PlayerTrapped");
        }

        if (!other.gameObject.CompareTag("Terrain") && !other.gameObject.CompareTag("Enemy") && !other.gameObject.CompareTag("Player") && !other.gameObject.name.Equals("NoiseRange") && !other.gameObject.name.Equals("TrapSphereCollider") && !other.gameObject.name.Equals("Beartrap") && !trappedCooldown) 
        {
            Debug.Log(other.gameObject.name + " has activated trap");
            StartCoroutine("TrapTriggered");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && isTrapped)
        {
            isTrapped = false;
        }
    }

    public IEnumerator TrapTriggered()
    {

        //play animation
        anim.SetBool("trigger", true);
        //playsound
        PlaySound();
        //play particle effect
        blood.Play();
        //immobilize trap
        trappedCooldown = true;
        yield return new WaitForSeconds(1f);
        interactable.enabled = true;
    }    

    public IEnumerator PlayerTrapped()
    {
        if(playerSpeed.isTrapped == true)
        {
            StartCoroutine("TeleportEnemyTimerSpawn");
            //StartCoroutine("TeleportEnemyTimer"); // bring enemy nearby to investigate trap
        }
        StartCoroutine("TeleportEnemyTimerSpawn");
        Debug.Log("Player trapped");
        // stop player speed
        playerSpeed.isTrapped = true;
        playerSpeed.movementSpeed = playerSpeed.stoppedSpeed;

        //play beartrap animation
        anim.SetBool("trigger", true);        

        //alert nearby enemies
        int layerMask = 1 << 6; //Layer 11
        GameObject scHolder = new GameObject();
        scHolder.transform.position = gameObject.transform.position;
        Vector3 scHolderPos = scHolder.transform.localPosition;
        scHolder.name = "TrapSphereCollider";
        SphereCollider sc = scHolder.AddComponent(typeof(SphereCollider)) as SphereCollider; //CREATE COLLIDER 
        sc.isTrigger = true; // SET AS TRIGGER
        sc.radius = noiseRadius; //SET RADIUS OF TRIGGER

        Collider[] EnemyColliders = Physics.OverlapSphere(scHolderPos, sc.radius, layerMask);//GET COLLIDERS WITHIN TRIGGER RADIUS - IGNORE ANYTHING NOT HIDINGSPOT LAYER

        if (EnemyColliders.Length > 0)
        {
            int RandomEnemy = Random.Range(0, EnemyColliders.Length); // ROLL FOR RANDOM ENEMY IN THE ARRAY

            Enemy RandomEnemyToInvestigate = EnemyColliders[RandomEnemy].GetComponent<Enemy>(); // GET THE ENEMY SCRIPT IN GAME WORLD OF RANDOM ENEMY

            teleportedEnemy = RandomEnemyToInvestigate.gameObject;

            //RandomEnemyToInvestigate.BroadcastMessage("Investigate"); // this was set to ATtack, why?
            if (RandomEnemyToInvestigate.startIdleEnemy == false)
            {
                RandomEnemyToInvestigate.startIdleEnemy = true;
            }

            if (RandomEnemyToInvestigate.detectedEnemy == false)
            {
                SpawnEnemyAtTrapSpawnpoint();

                RandomEnemyToInvestigate.investigatingEvidence = true;
                RandomEnemyToInvestigate.movingToEvidence = true;
                RandomEnemyToInvestigate.InvestigateLocation = gameObject;
            }
            Debug.Log(RandomEnemyToInvestigate.gameObject.name + " is investigating "+ gameObject.name);
            //sc.radius = 0; // soft disable collider
            Destroy(scHolder);
        }
        //playsound
        PlaySound();
        //play particle effect
        blood.Play();

        yield return new WaitForSeconds(trappedTime);
        anim.SetBool("trigger", false);
        playerSpeed.isTrapped = false;
        playerSpeed.movementSpeed = playerSpeed.defaultMovementSpeed; // reset movement speed once timer reaches 0
        StartCoroutine("PlayerTrappedCooldown");
        Debug.Log("Player released");
    }

    private void SpawnEnemyAtTrapSpawnpoint()
    {
        teleportedEnemy.transform.parent.position = enemySpawnPoint.transform.position;
    }

    public IEnumerator PlayerTrappedCooldown()
    {
        trappedCooldown = true;
        yield return new WaitForSeconds(trappedCooldownTime);

        trappedCooldown = false;
    }

    void PlaySound()
    {
        int index;

        index = Random.Range(0, bearTrapSound.Length);

        aud.clip = bearTrapSound[index];

        aud.Play();
    }

    void ResetTrap()
    {
        PlaySound();

        anim.SetBool("trigger", false);

        trapTriggered = false;
        trappedCooldown = false;
        deployed = true;

        interactable.enabled = false;
    }
}
