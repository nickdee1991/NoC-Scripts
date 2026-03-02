using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectiveParent : MonoBehaviour
{
    public bool IsLevelObjective; // this ObjectiveParent will advance the player to the next level
    public bool MoveToNextLevel;
    public bool SpawnsNewItem;
    public bool DisableColliderAfterComplete;
    public string AlternateLevelName;

    private GameManager gm;
    private AudioManager aud;
    private InventoryManager invMan;
    private UIManager UImanager;

    private TextMeshProUGUI text;
    private Animator textAnim;
    public Animator parentAnim;
    [SerializeField]
    private Animation anim;

    private MeshRenderer mesh;


    public GameObject[] Objectives;

    public GameObject[] objectivesToEnable; //enables objects to show player is completing objective eg. hook and rope for 3Town wall

    private GameObject player;
    public GameObject SpawnableItem;
    public GameObject ObjectMesh;

    private float ObjectiveTimerWait = 1.5f;

    public string SoundToPlay; // sound for when objectiveParent is activated

    public string ObjectiveCompleteText;
    public string ObjectName;

    public int ObjectivesComplete;
    public int ObjectivesToComplete;

    public bool hasItem;
    private bool hasSpawnedItem;
    public bool hasCompletedObjective;
    public bool playerInRange;

    private int destPoint;

    public Transform[] spawnPoints;

    void Start()
    {
        GameObject clone;
        ObjectivesComplete = 0;
        ObjectivesToComplete = Objectives.Length;

        player = GameObject.FindGameObjectWithTag("Player");
        text = GameObject.FindGameObjectWithTag("UItext").GetComponentInChildren<TextMeshProUGUI>();
        textAnim = GameObject.FindGameObjectWithTag("UItext").GetComponentInChildren<Animator>();
        invMan = FindObjectOfType<InventoryManager>();

        aud = FindObjectOfType<AudioManager>();
        UImanager = FindObjectOfType<UIManager>();
        gm = FindObjectOfType<GameManager>();

        playerInRange = false;
        hasSpawnedItem = false;
        hasCompletedObjective = false;
        mesh = GetComponent<MeshRenderer>();
        if (ObjectMesh == false)
        {
            mesh.enabled = true;
        }
        else
        {
            mesh.enabled = false;

            clone = Instantiate(ObjectMesh, gameObject.transform);
            parentAnim = clone.GetComponent<Animator>();
        }

        destPoint = (Random.Range(0, spawnPoints.Length));

        if (spawnPoints.Length != 0)
        {
            transform.position = spawnPoints[destPoint].position;
            transform.rotation = spawnPoints[destPoint].rotation;
        }
    }

    public void ObjectivesCheck()
    {
        if (playerInRange && hasItem && !hasCompletedObjective  && (Input.GetMouseButtonDown(0)))//&& invMan.inventoryOpen == true  
        {
            ObjectiveComplete();
        }
    }

    public void ObjectiveComplete()
    {
        if (ObjectivesComplete >= ObjectivesToComplete)
        {
            if (SpawnsNewItem && hasSpawnedItem == false)
            {
                ObjectiveSpawnItem();
            }

            if (DisableColliderAfterComplete)
            {
                GetComponent<BoxCollider>().enabled = false;
            }

            if (IsLevelObjective == true)
            {
                if (MoveToNextLevel)
                {
                    gm.LevelComplete();
                }
                else
                {
                    gm.AlternateLevelName = AlternateLevelName;
                    gm.AlternateLevel();
                }
            }

            if(objectivesToEnable.Length > 0)
            {
                foreach (GameObject objectEnable in objectivesToEnable)
                {
                    objectEnable.SetActive(true);
                }
            }

            player.GetComponent<PlayerSimpleMovement>().isInteracting = true;

            //check for item in inventory
            UImanager.UIObjectEnable();
            text.text = ObjectiveCompleteText;        //activate UI
            aud.PlaySound(SoundToPlay);//play sound
            if(parentAnim != null)
            {
                parentAnim.SetBool("objectivecomplete", true); // play animation
            }
            GetComponentInChildren<ParticleSystem>().Play();//play particle effect?
            StartCoroutine("ObjectiveTimer"); // timer to allow the particle system to play


            hasCompletedObjective = true;
        }
        else
        {
            aud.PlaySound("DoorLocked");
        }
    }

    public void ObjectiveSpawnItem()
    {
        //spawn item
        SpawnableItem.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y +0.25f, gameObject.transform.position.z -0.5f);
        if (SpawnableItem.GetComponent<Rigidbody>() == null)
        {
            SpawnableItem.AddComponent<Rigidbody>();
        }

        SpawnableItem.GetComponent<Rigidbody>().AddForce(transform.up * 250); // shoot it out of the Objectiveparent
        hasSpawnedItem = true;
        Debug.Log("Item " + SpawnableItem.name + " spawned");
    }

    public IEnumerator ObjectiveTimer()
    {
        yield return new WaitForSeconds(ObjectiveTimerWait);
        Debug.Log("Objective "+ ObjectName + " complete");
    }
}
