using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    //container for objective prefabs
    public GameObject[] ObjectiveContainer;
    public Transform objectInteractPosition;

    public MeshRenderer[] BagMesh;

    public int inventorySelection = 0;

    public GameObject Slot1;
    public GameObject Slot2;

    public GameObject SelectedItem;
    private Transform Player;

    public TextMeshProUGUI text;
    //public GameObject itemText; //TextMeshPro
    private Animator textAnim;
    private Animator bagAnim;

    private UIManager uiManager;

    public bool droppedInventory;
    public bool inventoryFull;
    public bool inventoryOpen;
    [SerializeField]
    private float throwForce;
    public bool pickupCooldown;

    void Start()
    {

        objectInteractPosition = GameObject.Find("ObjectInteractPosition").transform;
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        uiManager = FindObjectOfType<UIManager>();
        bagAnim = GetComponentInParent<Animator>();
        text = uiManager.objectText;
        //itemText = GameObject.Find("ObjectText");//GameObject.Find("ItemDescription").GetComponentInChildren<TextMeshPro>(); // should this be a thing? why did we make UI Manager if were doing this shit?
        textAnim = GameObject.FindGameObjectWithTag("UItext").GetComponentInChildren<Animator>();

        inventoryFull = false;
        pickupCooldown = false;
    }

    public IEnumerator PickupCooldown()
    {
        yield return new WaitForSeconds(.5f);
        pickupCooldown = false;
    }


    private void Update()
    {
        if (inventoryOpen)
        {

            if (Input.GetKeyDown(KeyCode.A))            //cycle through items in inventory with A & D
            {

                inventorySelection = 1;
                InventorySelect();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                inventorySelection = 2;
                InventorySelect();
            }

            if (Input.GetKeyDown(KeyCode.G))            //pressing G will drop item (eg a trap, a noisemaker)
            {
                inventorySelection = 0;

                if (SelectedItem != null)
                {
                    DropItem();
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                inventorySelection = 0;

                if (SelectedItem != null)
                {
                    ThrowItem();
                }
            }

            if (Input.GetMouseButtonDown(0) && inventorySelection >0 && SelectedItem!= null)
            {
                UseSelectedItem();
            }
        }
    }

    public void UseSelectedItem()
    {
        Item objChild = SelectedItem.GetComponent<Item>();
        switch (objChild.thisObjectType)
        {
            case Item.ObjectType.OBJECTIVECHILD:

                StartCoroutine("ShowItemDescription");

                bool ArrayContains(GameObject[] array, GameObject g)
                {
                    for (int i = 0; i < objChild.Parent[0].Objectives.Length; i++)
                    {
                        if (array[i] == g) return true;
                    }
                    return false;
                }

                if (ArrayContains(objChild.Parent[0].Objectives, SelectedItem) == true) //good bool for checking an object in array
                {
                    foreach (ObjectiveParent objP in objChild.Parent)
                    {
                        objP.hasItem = true;

                        if (objP.playerInRange && objP.ObjectivesComplete < objP.ObjectivesToComplete)
                        {
                            objP.ObjectivesComplete++;
                            objP.ObjectivesCheck();
                            if (SelectedItem.GetComponent<Item>().SoundToPlay != null)
                            {
                                SelectedItem.GetComponent<Item>().aud.PlaySound(SelectedItem.GetComponent<Item>().SoundToPlay);
                            }
                            if (objChild.itemDestroyedAfterUse)
                            {
                                objChild.gameObject.transform.parent = null;
                                objChild.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 20, transform.position.z);
                            }
                        }
                        else
                        {
                            StartCoroutine("ShowItemDescription");
                        }
                    }
                }
                break;

            case Item.ObjectType.WORLDOBJECT:
                if (SelectedItem.GetComponent<Item>().ObjectPickedUpText !=null) 
                {
                    StartCoroutine("ShowItemDescription");
                }                
                break;

            case Item.ObjectType.LOREOBJECT:
                InventoryLoreObject();
                if (SelectedItem.GetComponent<Item>().SoundToPlay != null)
                {
                    SelectedItem.GetComponent<Item>().aud.PlaySound(SelectedItem.GetComponent<Item>().SoundToPlay);
                }
                break;

        }

    }

    public IEnumerator ShowItemDescription()
    {
        Debug.Log("item not in range , show description");
        text.text = SelectedItem.GetComponent<Item>().ObjectPickedUpText;
        uiManager.UIObjectEnable();
        yield return new WaitForSeconds(2f);
        uiManager.UIObjectDisable();
    }

    public void DropItem()
    {
        SelectedItem.GetComponent<Item>().objCollider.enabled = false;
        SelectedItem.transform.SetParent(null);
        SelectedItem.transform.position = new Vector3(objectInteractPosition.transform.position.x, objectInteractPosition.transform.position.y, objectInteractPosition.transform.position.z); // + 1f

        if (SelectedItem.GetComponent<Item>().SoundToPlay != null)
        {
            SelectedItem.GetComponent<Item>().aud.PlaySound(SelectedItem.GetComponent<Item>().SoundToPlay);
        }

        SelectedItem.GetComponent<Item>().objCollider.enabled = true;
        SelectedItem.GetComponent<Item>().hasPickedUp = false;
        SelectedItem.GetComponent<Item>().twinkleParticle.Play();
        SelectedItem.AddComponent<Rigidbody>();
        uiManager.StopUILoreObject();
        SelectedItem = null;
        inventoryFull = false;
        bagAnim.SetBool("isInteracting",true);
        StartCoroutine("InteractingAnimationCooldown");
    }

    public void ThrowItem()
    {
        SelectedItem.GetComponent<Item>().objCollider.enabled = false;
        SelectedItem.transform.SetParent(null); // this reverts to 0,0,0 for some reason
        SelectedItem.transform.position = new Vector3 (objectInteractPosition.transform.position.x, objectInteractPosition.transform.position.y, objectInteractPosition.transform.position.z);//  + 1f move the item to the front of player

        if (SelectedItem.GetComponent<Item>().SoundToPlay != null)
        {
            SelectedItem.GetComponent<Item>().aud.PlaySound(SelectedItem.GetComponent<Item>().SoundToPlay);
        }

        SelectedItem.transform.Rotate(Vector3.up * Random.Range(0, 100) * Random.Range(0, 100));
        SelectedItem.AddComponent<Rigidbody>();
        SelectedItem.GetComponent<Rigidbody>().AddForce(objectInteractPosition.transform.forward * throwForce); // push the item from main camera position
        uiManager.StopUILoreObject();

        SelectedItem.GetComponent<Item>().objCollider.enabled = true;
        SelectedItem.GetComponent<Item>().hasPickedUp = false;
        SelectedItem.GetComponent<Item>().twinkleParticle.Play(); // play effect so player can spot

        SelectedItem = null;
        inventoryFull = false;
        bagAnim.SetBool("isInteracting", true);
        StartCoroutine("InteractingAnimationCooldown");
    }

    public void InventoryCheck()
    {     
        if (Slot1.transform.childCount > 0 && Slot2.transform.childCount > 0)
        {
            //inventory full
            inventoryFull = true;

        }else{
            inventoryFull = false;
        }
    }

    public IEnumerator InventoryFull()
    {
        uiManager.UIObjectEnable();
        text.text = "I dropped my bag when they grabbed me";
        yield return new WaitForSeconds(2f);
        uiManager.UIObjectDisable();
    }

    IEnumerator InteractingAnimationCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        bagAnim.SetBool("isInteracting", false);
    }
    IEnumerator InventoryCloseCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        bagAnim.SetBool("isInteracting", false);
    }

    public void InventoryLoreObject()
    {    
        //call uimanager
        uiManager.loreObject = SelectedItem.GetComponent<Item>();
        uiManager.UILoreObject();
    }

        public void InventorySelect()
    {
        switch (inventorySelection) {

            case 1:            //highlight item 1

                //inventory 1
                if (Slot1.transform.childCount >0)
                {
                    SelectedItem = Slot1.transform.GetChild(0).gameObject;
                    SelectedItem.GetComponent<Item>().objCollider.enabled = false;
                    Slot1.transform.GetChild(0).DOLocalMove(new Vector3(0, 0.5f, 0), .15f);
                    bagAnim.SetBool("CheckItemInventory",true);
                    bagAnim.SetBool("OpenInventory", false);
                   // text.text = SelectedItem.GetComponent<Item>().ObjectName;
                   // text.gameObject.SetActive(true);

                }

                if (Slot2.transform.childCount > 0)
                {
                    Slot2.transform.GetChild(0).DOMove(new Vector3(Slot2.transform.position.x, Slot2.transform.position.y, Slot2.transform.position.z), .15f);

                    if (Slot2.transform.GetChild(0).GetComponent<Item>().thisObjectType == Item.ObjectType.LOREOBJECT)
                    {
                        uiManager.StopUILoreObject();
                    }
                }
                break;

            case 2:
                //inventory 2
                if(Slot2.transform.childCount > 0)
                {
                    SelectedItem = Slot2.transform.GetChild(0).gameObject;
                    SelectedItem.GetComponent<Item>().objCollider.enabled = false;
                    Slot2.transform.GetChild(0).DOLocalMove(new Vector3(0, 0.5f, 0), .15f);
                    bagAnim.SetBool("CheckItemInventory", true);
                    bagAnim.SetBool("OpenInventory", false);
                   // text.text = SelectedItem.GetComponent<Item>().ObjectName;
                   // text.gameObject.SetActive(true);
                }

                if (Slot1.transform.childCount > 0)
                {
                    Slot1.transform.GetChild(0).DOMove(new Vector3(Slot1.transform.position.x, Slot1.transform.position.y, Slot1.transform.position.z), .15f);

                    if (Slot1.transform.GetChild(0).GetComponent<Item>().thisObjectType == Item.ObjectType.LOREOBJECT)
                    {
                        uiManager.StopUILoreObject();
                    }
                }
                break;

            case 0:
                inventorySelection = 0;

                if (Slot1.transform.childCount > 0)
                {
                    Slot1.transform.GetChild(0).transform.position = Slot1.transform.position;
                }
                if (Slot2.transform.childCount > 0)
                {
                    Slot2.transform.GetChild(0).transform.position = Slot2.transform.position;
                }
                //text.enabled = false;
                uiManager.StopUILoreObject();
                SelectedItem = null;
                break;
        }
    }
}
