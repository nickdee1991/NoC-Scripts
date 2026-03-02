using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class DeployableItem : MonoBehaviour
{
    private GameObject player;
    private GameManager gm;
    public GameObject ObjectMesh;

    private TextMeshProUGUI text;
    private Animator textAnim;
    private UIManager UImanager;
    private MeshRenderer mesh;
    private InventoryManager invManager;

    public string ObjectName;
    public string ObjectPickedUpText;
    public ObjectiveParent[] Parent;
    public Animator ObjectiveAnim; // an animation to play when objchild is brought to parent
    public ParticleSystem twinkleParticle;

    private bool multipleObjectives;
    public bool itemDestroyedAfterUse; // will this gameobject be deleted once used?
    public bool hasPickedUp;
    private bool playerInRange;

    private float ObjectiveTimerWait = 0.5f;

    public float textFadeTime;
    private int destPoint;

    public Transform[] spawnPoints;
    private Transform InventoryPosition;

    private void Awake()
    {

        invManager = FindObjectOfType<InventoryManager>();
        mesh = GetComponent<MeshRenderer>();

        if (ObjectMesh == false)
        {
            mesh.enabled = true;
        }else{
            mesh.enabled = false;
            Instantiate(ObjectMesh, gameObject.transform);
        }

        gm = FindObjectOfType<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        text = GameObject.FindGameObjectWithTag("UItext").GetComponentInChildren<TextMeshProUGUI>();
        textAnim = GameObject.FindGameObjectWithTag("UItext").GetComponentInChildren<Animator>();
    }

    void Start()
    {
        //InventoryPosition = GameObject.Find("Objective_Holder").transform;
        InventoryPosition = invManager.Slot1.transform;

        //Debug.Log("InventoryPosition = " + InventoryPosition);

            //destPoint is assigned a random number = to the length of the spawnpoint array
            destPoint = (Random.Range(0, spawnPoints.Length));
        UImanager = FindObjectOfType<UIManager>();

        //Instantiate item in random location on start
        if (spawnPoints.Length != 0)
        {
            transform.position = spawnPoints[destPoint].position;
        }

        hasPickedUp = false;
    }

    private void FixedUpdate()
    {
        if (playerInRange) // hasPickedup == false;
        {
            invManager.InventoryCheck();
            //playerInRange = true;
            if (Input.GetMouseButtonDown(0) && hasPickedUp == false)
            {
                PickupItem();
            }
        }
    }

    #region ObjectiveSelect/Pickup

    public IEnumerator TextFadeIn()
    {
        text.enabled = true;
        textAnim.SetBool("fadeText", true);
        //text.material.DOFade(255,5);
        yield return new WaitForSeconds(textFadeTime);
    }

    public IEnumerator TextFadeOut()
    {
        //text.material.DOFade(0, 5);
        textAnim.SetBool("fadeText", false);
        yield return new WaitForSeconds(textFadeTime);
        text.text = "";
        text.enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            text.enabled = true;
            playerInRange = true;
            //Debug.Log(playerInRange);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        StartCoroutine("TextFadeOut");
        playerInRange = false;
    }

    private void OnMouseExit()
    {
        //playerInRange = false;
        StartCoroutine("TextFadeOut");
        UImanager.PlayerCrosshairState = UImanager.PlayerCrosshairStateSprites[0];
    }

    private void OnMouseEnter()
    {
        if (playerInRange)
        {
            StartCoroutine("TextFadeIn");
            Debug.Log(ObjectName);
            text.text = ObjectName;
            UImanager.PlayerCrosshairState = UImanager.PlayerCrosshairStateSprites[1];
        }
    }

    public void PickupItem()
    {
        if (invManager.inventoryFull == false)
        {
            if (ObjectiveAnim != null)
            {
                ObjectiveAnim.SetBool("ObjectiveAnim", true);//add optional animation to play
            }

            if (invManager.Slot1.transform.childCount > 0)
            {
                InventoryPosition = invManager.Slot2.transform;
            }

            player.GetComponent<PlayerSimpleMovement>().isInteracting = true;

            Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.transform.DOMove(player.transform.position, 0.5f);
            twinkleParticle.Stop();
            GetComponent<BoxCollider>().enabled = false;
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
            StartCoroutine("ObjectiveTimer");// timer to allow the particle system to play
        }
        else
        {
            text.enabled = true;
            text.text = "Inventory Full";
        }
    }
    #endregion

    public IEnumerator ObjectiveTimer()
    {
        if(ObjectPickedUpText != null)
        {
            text.text = ObjectPickedUpText;
        } else {
            text.text = ObjectName + " picked up"; //text = " picked up"
        }

        StartCoroutine("TextFadeIn");
        yield return new WaitForSeconds(ObjectiveTimerWait);
        GetComponentInChildren<ParticleSystem>().Stop();
        transform.parent = InventoryPosition.transform;
        gameObject.transform.position = InventoryPosition.transform.position; //pickup this object
        gameObject.transform.rotation = InventoryPosition.transform.rotation; //pickup this object
    }


}
