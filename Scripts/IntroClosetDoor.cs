using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IntroClosetDoor : MonoBehaviour
{
    private Animator anim;
    //private bool playerInRange;
    private bool doorOpened;

    private AudioManager aud;

    public ObjectiveParent closetObjective;

    private TextMeshProUGUI text;
    private Animator textAnim;

    public string ObjectName;
    public float textFadeTime = .5f;

    private void Start()
    {
        doorOpened = false;

        aud = FindObjectOfType<AudioManager>();

        anim = GetComponent<Animator>();
        text = GameObject.FindGameObjectWithTag("UItext").GetComponentInChildren<TextMeshProUGUI>();
        textAnim = GameObject.FindGameObjectWithTag("UItext").GetComponentInChildren<Animator>();
    }

    public void Update()
    {
        if(closetObjective.hasCompletedObjective == true&&doorOpened == false)
        {
            StartCoroutine("OpenClosetDoor");
        }
    }

    IEnumerator OpenClosetDoor()
    {
        Debug.Log("playerinrange");
        yield return new WaitForSeconds(1f);
        aud.PlaySound("ClosetOpen");
        doorOpened = true;
        anim.SetTrigger("introclosetdoor");
        if (Input.GetKeyDown(KeyCode.E))
        {
            aud.PlaySound("ClosetOpen");
            doorOpened = true;
            anim.SetTrigger("introclosetdoor");
        }
    }
}
