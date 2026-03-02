using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class WorldItem : MonoBehaviour
{
    private GameObject player;
    private GameManager gm;
    public GameObject ObjectMesh;

    public enum ObjectType { OBJECTIVECHILD, WORLDOBJECT, DEPLOYABLEOBJECT, LOREOBJECT }
    public ObjectType thisObjectType;

    private TextMeshProUGUI text;
    private Animator textAnim;
    private UIManager UImanager;
    private MeshRenderer mesh;

    public string ObjectName;
    public string ObjectPickedUpText;
    public ParticleSystem twinkleParticle;

    private InventoryManager invManager;
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
            switch (thisObjectType)
            {
                case ObjectType.OBJECTIVECHILD:
                    //stuff
                    break;
                case ObjectType.WORLDOBJECT:
                    //stuff

                    if (invManager.Slot1.transform.childCount > 0)
                    {
                        InventoryPosition = invManager.Slot2.transform;
                    }

                    player.GetComponent<PlayerSimpleMovement>().isInteracting = true;

                    Destroy(gameObject.GetComponent<Rigidbody>());
                    gameObject.transform.DOMove(player.transform.position, 0.5f);
                    twinkleParticle.Stop();
                    GetComponent<MeshCollider>().enabled = false;
                    hasPickedUp = true;

                    //Parent.hasItem = true; //update in gamemanager
                    GetComponentInChildren<ParticleSystem>().Play();
                    StartCoroutine("WorldItemTimer");// timer to allow the particle system to play    

            break;
                case ObjectType.DEPLOYABLEOBJECT:
                    //stuff

                    if (invManager.Slot1.transform.childCount > 0)
                    {
                        InventoryPosition = invManager.Slot2.transform;
                    }

                    player.GetComponent<PlayerSimpleMovement>().isInteracting = true;

                    Destroy(gameObject.GetComponent<Rigidbody>());
                    gameObject.transform.DOMove(player.transform.position, 0.5f);
                    twinkleParticle.Stop();
                    GetComponent<MeshCollider>().enabled = false;
                    hasPickedUp = true;

                    //Parent.hasItem = true; //update in gamemanager
                    GetComponentInChildren<ParticleSystem>().Play();
                    StartCoroutine("WorldItemTimer");// timer to allow the particle system to play  

                    break;
                case ObjectType.LOREOBJECT:
                    //stuff
                    break;

            }
        }else{
            text.enabled = true;
            text.text = "Inventory Full";
        }

    }
    #endregion

    public IEnumerator WorldItemTimer()
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
