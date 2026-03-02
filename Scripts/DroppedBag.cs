using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class DroppedBag : MonoBehaviour
{
    private GameObject player;
    private GameManager gm;

    private TextMeshProUGUI text;
    private Animator textAnim;
    private UIManager UImanager;
    private InventoryManager invManager;

    public string ObjectName;
    public string ObjectPickedUpText;
    public ParticleSystem twinkleParticle;

    public bool hasPickedUp;
    private bool playerInRange;

    //private float ObjectiveTimerWait = 0.5f;

    public float textFadeTime;

    private void Awake()
    {
        invManager = FindObjectOfType<InventoryManager>();

        gm = FindObjectOfType<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        text = GameObject.FindGameObjectWithTag("UItext").GetComponentInChildren<TextMeshProUGUI>();
        textAnim = GameObject.FindGameObjectWithTag("UItext").GetComponentInChildren<Animator>();
    }

    void Start()
    {
        UImanager = FindObjectOfType<UIManager>();
        hasPickedUp = false;
    }

    #region
    void PickupBag()
    {
        invManager.droppedInventory = false;
        Destroy(gameObject.GetComponent<Rigidbody>());
        gameObject.transform.DOMove(player.transform.position, 0.5f);
        twinkleParticle.Stop();
        GetComponent<SphereCollider>().enabled = false;
        hasPickedUp = true;
        GetComponentInChildren<ParticleSystem>().Play();
        StartCoroutine("ObjectiveTimer");// timer to allow the particle system to play
    }
    #endregion

    #region ObjectiveSelect/Pickup



    public IEnumerator ObjectiveTimer()
    {
        yield return new WaitForSeconds(textFadeTime);
        GetComponentInChildren<ParticleSystem>().Stop();
        Destroy(gameObject);
    }

    private void OnMouseExit()
    {
        //playerInRange = false;
        StartCoroutine("TextFadeOut");
        UImanager.PlayerCrosshairState = UImanager.PlayerCrosshairStateSprites[0];
    }

    private void OnMouseEnter()
    {
        Debug.Log(ObjectName);
        //text.text = ObjectName;
        if (playerInRange)
        {
            StartCoroutine("TextFadeIn");
            UImanager.PlayerCrosshairState = UImanager.PlayerCrosshairStateSprites[1];
        }
    }
}


    #endregion
