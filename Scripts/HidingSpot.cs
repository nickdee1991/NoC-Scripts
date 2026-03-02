using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HidingSpot : MonoBehaviour
{
    private AudioSource aud;
    private UIManager UImanager;
    [SerializeField]
    private GameObject Player;
    [SerializeField]
    private GameObject hidingCamera;
    [SerializeField]
    private bool inHidingSpot;
    [SerializeField]
    private Animator anim;

    private TextMeshProUGUI text;
    [SerializeField]
    public string ObjectName = "Hold E To Hide"; //used for CameraLook

    [SerializeField]
    private string hidingSpotExitText = "Exit";

    public bool playerInRange;

    public float hidingSpotDelay = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        aud = GetComponent<AudioSource>();
        UImanager = FindObjectOfType<UIManager>();
        Player = GameObject.FindGameObjectWithTag("Player");
        text = UImanager.objectText;
        hidingCamera = GetComponentInChildren<Camera>().gameObject;
        hidingCamera.SetActive(false);
        inHidingSpot = false;
        playerInRange = false;

        if (anim == null)
        {
            anim = GetComponentInParent<Animator>();
        }
    }
    private void FixedUpdate()
    {
        UImanager.ObjectTime.gameObject.SetActive(false);
        if (inHidingSpot)
        {
            UImanager.UIObjectEnable();
            text.text = hidingSpotExitText;
            if (Input.GetKeyDown(KeyCode.G))
            {
                ExitedHidingSpot();
            }
        }

        if (hidingSpotDelay < 0.5f && !Input.GetKey(KeyCode.E))
        {
            hidingSpotDelay += Time.deltaTime;

        } // some clamping bullshit


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(anim == true)
            {
                anim.SetBool("Open", true);
            }

            if (Input.GetKey(KeyCode.E) && !inHidingSpot && !Player.GetComponentInChildren<InventoryManager>().inventoryOpen)
            {
                hidingSpotDelay -= Time.deltaTime;
                UImanager.ObjectTime.gameObject.SetActive(true);

                if (hidingSpotDelay <= 0)
                {
                    EnteredHidingSpot();
                    UImanager.ObjectTime.gameObject.SetActive(false);
                }
            }
        }

        if (other.gameObject.CompareTag("Enemy") && other.gameObject.GetComponent<Enemy>().searchingForPlayer == true)
        {
            Debug.Log("ENEMY activated hiding spot");
            if (inHidingSpot)
            {
                Debug.Log("ENEMY found player");
                ExitedHidingSpot();
            }            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (anim == true && anim.GetBool("Open"))
        {
            anim.SetBool("Open", false);
        }

        if (other.gameObject.CompareTag("Player")&& !inHidingSpot)
        {
            playerInRange = false;
        }
    }



    public void EnteredHidingSpot()
    {
        aud.volume = 0.25f;
        aud.Play();
        inHidingSpot = true;
        hidingCamera.SetActive(true);
        Player.SetActive(false);
    }

    public void ExitedHidingSpot()
    {
        aud.volume = 0.25f;
        aud.Play();
        inHidingSpot = false;
        hidingCamera.SetActive(false);
        Player.SetActive(true);

        FindObjectOfType<CameraLook>().StartCoroutine("RangeCooldown");
    }
}
