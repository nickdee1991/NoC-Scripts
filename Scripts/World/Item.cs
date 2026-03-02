using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class Item : MonoBehaviour
{
    public enum ObjectType { OBJECTIVECHILD, WORLDOBJECT, DEPLOYABLEOBJECT, LOREOBJECT }
    public ObjectType thisObjectType;

    private GameObject player;
    private GameManager gm;
    [SerializeField]
    public GameObject ObjectMesh;
    [SerializeField]
    public Collider objCollider;
    private TextMeshProUGUI text;
    [TextArea(3, 10)]
    public string loreTextText;

    private Animator textAnim;
    private UIManager UImanager;
    private MeshRenderer mesh;
    private InventoryManager invManager;
    public AudioManager aud;

    public string ObjectName;
    public string ObjectPickedUpText;
    public string SoundToPlay;
    public ObjectiveParent[] Parent;
    public Animator ObjectiveAnim; // an animation to play when objchild is brought to parent
    public ParticleSystem twinkleParticle;

    private bool multipleObjectives;
    public bool itemDestroyedAfterUse; // will this gameobject be deleted once used?
    public bool itemDestroyed;
    public bool hasPickedUp;
    public bool playerInRange;
    [SerializeField]
    private bool parentToSpawnPoint;

    [SerializeField]
    private float ObjectiveTimerWait = 2f;

    public float textFadeTime;
    private int destPoint;

    public Transform[] spawnPoints;
    public Vector3 defaultSpawnPoint; // if item doesn't randomly spawn, default spawnpoint will be set to there it was onsceneload (for returning items that fell out of bounds)
    private Transform InventoryPosition;
    public Transform objectInteractPosition;

    private void Awake()
    {
        itemDestroyed = false;
        invManager = FindObjectOfType<InventoryManager>();
        aud = FindObjectOfType<AudioManager>();
        mesh = GetComponent<MeshRenderer>();
        UImanager = FindObjectOfType<UIManager>();
        if (GameObject.Find("ObjectInteractPosition"))
        {
            objectInteractPosition = GameObject.Find("ObjectInteractPosition").transform;
        }


        if (ObjectMesh == false)
        {
            mesh.enabled = true;
        }else{
            mesh.enabled = false;
            GameObject go = Instantiate(ObjectMesh, gameObject.transform);
            objCollider = go.GetComponentInChildren<Collider>();
            Destroy(GetComponent<BoxCollider>());
        }

        gm = FindObjectOfType<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (GameObject.FindGameObjectWithTag("UItext"))
        {
            text = GameObject.Find("ObjectTextHolder").GetComponentInChildren<TextMeshProUGUI>(); // can this just call UImanager instead?
            textAnim = GameObject.FindGameObjectWithTag("UItext").GetComponentInChildren<Animator>();
        }

        twinkleParticle = GetComponentInChildren<ParticleSystem>();
    }

    void Start()
    {
        InventoryPosition = invManager.Slot1.transform;
        //destPoint is assigned a random number = to the length of the spawnpoint array
        destPoint = (Random.Range(0, spawnPoints.Length));
        if(spawnPoints.Length == 0)
        {
            defaultSpawnPoint = transform.position;
        }

        //Instantiate item in random location on start
        if (spawnPoints.Length != 0)
        {
            transform.position = spawnPoints[destPoint].position;
            transform.rotation = spawnPoints[destPoint].rotation;
            defaultSpawnPoint = transform.position;
            if (parentToSpawnPoint)
            {
                transform.parent = spawnPoints[destPoint].transform.parent;
                GetComponent<Rigidbody>().isKinematic = true;
            }
        }

        hasPickedUp = false;
        StartCoroutine("LateStart");
    }

    //spawn enemy only after room generation has finished
    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(.25f);
        if (objCollider != null)
        {
            objCollider.enabled = true;
        }

        StopCoroutine("LateStart");
    }

    private void Update()
    {
        if (playerInRange)
        {
            invManager.InventoryCheck();
            if (Input.GetMouseButtonDown(0) && hasPickedUp == false && !itemDestroyed)
            {
                PickupItem();
            }

            if (Input.GetKeyDown(KeyCode.E) && hasPickedUp == false && !itemDestroyed)
            {
                //Search child of item for Interactable script
                ActivateItemInteractable();
            }
        }

    }

    public void PickupItem()
    {
        if (invManager.inventoryFull == false && !invManager.pickupCooldown)
        {
            aud.PlaySound(SoundToPlay);
            invManager.pickupCooldown = true;
            invManager.StartCoroutine("PickupCooldown");
;            switch (thisObjectType)
            {
                case ObjectType.OBJECTIVECHILD:
                    //stuff
                    if (ObjectiveAnim != null)
                    {
                        ObjectiveAnim.SetBool("ObjectiveAnim", true);//add optional animation to play
                    }

                    if (invManager.Slot1.transform.childCount > 0)
                    {
                        InventoryPosition = invManager.Slot2.transform;
                        Debug.Log("PickupItem");
                    }

                    player.GetComponent<PlayerSimpleMovement>().isInteracting = true;

                    Destroy(gameObject.GetComponent<Rigidbody>());
                    objCollider.enabled = false;
                    gameObject.transform.DOMove(player.transform.position, 0.5f);
                    twinkleParticle.Stop();
                    hasPickedUp = true;

                    foreach (ObjectiveParent objP in Parent)
                    {
                        if (objP != null)
                        {
                            objP.hasItem = true;
                        }
                    }

                    //Parent.hasItem = true; //update in gamemanager
                    GetComponentInChildren<ParticleSystem>().Play();
                    StartCoroutine("ObjectiveTimer");
                    break;

                case ObjectType.WORLDOBJECT:
                    //stuff

                    if (invManager.Slot1.transform.childCount > 0)
                    {
                        InventoryPosition = invManager.Slot2.transform;
                    }

                    player.GetComponent<PlayerSimpleMovement>().isInteracting = true;
                    Destroy(gameObject.GetComponent<Rigidbody>());
                    objCollider.enabled = false;
                    Debug.Log(objCollider.name + " enabled is "+ objCollider.enabled );
                    gameObject.transform.DOMove(player.transform.position, 0.5f);
                    twinkleParticle.Stop();
                    hasPickedUp = true;

                    //Parent.hasItem = true; //update in gamemanager
                    GetComponentInChildren<ParticleSystem>().Play();
                    StartCoroutine("WorldObjectTimer");// timer to allow the particle system to play    

                    break;


                case ObjectType.LOREOBJECT:
                    //stuff

                    if (invManager.Slot1.transform.childCount > 0)
                    {
                        InventoryPosition = invManager.Slot2.transform;
                    }

                    //dislay lore text on screen on mouse over + activatebutton
                    player.GetComponent<PlayerSimpleMovement>().isInteracting = true;
                    Destroy(gameObject.GetComponent<Rigidbody>());
                    objCollider.enabled = false;
                    Debug.Log(objCollider.name + " enabled is " + objCollider.enabled);
                    gameObject.transform.DOMove(player.transform.position, 0.5f);
                    twinkleParticle.Stop();
                    hasPickedUp = true;
                    GetComponentInChildren<ParticleSystem>().Play();
                    StartCoroutine("LoreObjectTimer");// timer to allow the particle system to play
                    break;

                case ObjectType.DEPLOYABLEOBJECT:

                    if (invManager.Slot1.transform.childCount > 0)
                    {
                        InventoryPosition = invManager.Slot2.transform;
                    }

                    //dislay lore text on screen on mouse over + activatebutton
                    player.GetComponent<PlayerSimpleMovement>().isInteracting = true;
                    Destroy(gameObject.GetComponent<Rigidbody>());
                    objCollider.enabled = false;
                    Debug.Log(objCollider.name + " enabled is " + objCollider.enabled);
                    gameObject.transform.DOMove(player.transform.position, 0.5f);
                    twinkleParticle.Stop();
                    hasPickedUp = true;
                    GetComponentInChildren<ParticleSystem>().Play();
                    StartCoroutine("DeployableTimer");// timer to allow the particle system to play
                    break;
            }
        }
        else
        {
            UImanager.UIObjectEnable();
            text.text = "Inventory Full";
            Debug.Log("Inventory full " + invManager.inventoryFull);
            UImanager.loreObject = null;
        }
    }

    public void ActivateItemInteractable()
    {
        Interactable interactable = GetComponentInChildren<Interactable>();

        interactable.ActivateInteractable();
    }

    public IEnumerator ObjectiveTimer()
    {
        if(ObjectPickedUpText != null)
        {
            UImanager.UIObjectEnable();
            text.text = ObjectPickedUpText;
        } else {
            UImanager.UIObjectEnable();
            text.text = ObjectName + " picked up"; //text = " picked up"
        }
        yield return new WaitForSeconds(ObjectiveTimerWait);
        UImanager.UIObjectDisable();
        GetComponentInChildren<ParticleSystem>().Stop();
        transform.parent = InventoryPosition.transform;
        gameObject.transform.position = InventoryPosition.transform.position; //pickup this object
        gameObject.transform.rotation = InventoryPosition.transform.rotation; //pickup this object
    }

    public IEnumerator WorldObjectTimer()
    {
        if (ObjectPickedUpText != null)
        {
            UImanager.UIObjectEnable();
            text.text = ObjectPickedUpText;
        }
        else
        {
            UImanager.UIObjectEnable();
            text.text = ObjectName + " picked up"; //text = " picked up"
        }
        yield return new WaitForSeconds(ObjectiveTimerWait);
        GetComponentInChildren<ParticleSystem>().Stop();
        UImanager.UIObjectDisable();
        transform.parent = InventoryPosition.transform;
        gameObject.transform.position = InventoryPosition.transform.position; //pickup this object
        gameObject.transform.rotation = InventoryPosition.transform.rotation; //pickup this object
    }

    public IEnumerator LoreObjectTimer()
    {
        if (ObjectPickedUpText != null)
        {
            text.text = ObjectPickedUpText;
        }
        else
        {
            text.text = ObjectName + " picked up"; //text = " picked up"
        }
        yield return new WaitForSeconds(ObjectiveTimerWait);
        GetComponentInChildren<ParticleSystem>().Stop();
        transform.parent = InventoryPosition.transform;
        gameObject.transform.position = InventoryPosition.transform.position; //pickup this object
        gameObject.transform.rotation = InventoryPosition.transform.rotation; //pickup this object
    }

    public IEnumerator DeployableTimer()
    {
        if (ObjectPickedUpText != null)
        {
            UImanager.UIObjectEnable();
            text.text = ObjectPickedUpText;
        }
        else
        {
            UImanager.UIObjectEnable();
            text.text = ObjectName + " picked up"; //text = " picked up"
        }
        yield return new WaitForSeconds(ObjectiveTimerWait);
        GetComponentInChildren<ParticleSystem>().Stop();
        transform.parent = InventoryPosition.transform;
        gameObject.transform.position = InventoryPosition.transform.position; //pickup this object
        gameObject.transform.rotation = InventoryPosition.transform.rotation; //pickup this object
    }
}
