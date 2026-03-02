using System.Collections;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string ObjectName; // what will UI text display this object as
    [SerializeField]
    private GameObject interactableObject; // object that is attached to this switch

    [SerializeField]
    private string scriptToInteractWith; // type what script the player will interact with.

    [SerializeField]
    private string methodInsideInteractedScript; // the name for the method you want to activate in the object
    [SerializeField]
    private Animator anim;

    private AudioManager aud;
    [SerializeField]
    private string soundToPlay; // sound to play
    [SerializeField]
    public bool playerInRange;
    [HideInInspector]
    public bool activateInteractable; // the switch that is activated by the player
    [SerializeField]
    private bool canRepeatAudio;
    private bool audioPlayed;

    [SerializeField]
    private bool overrideStart;

    [SerializeField]
    private bool playOnTrigger; // no prompt from player needed

    [SerializeField]
    private float timeToWait = .1f; // for coroutine timer - would need to be changeable to account for different interactables

    private void Start()
    {
        aud = FindObjectOfType<AudioManager>();
        if (GameObject.Find("Main Camera"))
        {
            anim = GameObject.Find("Main Camera").GetComponent<Animator>();
        }

    }

    private void Update()
    {
        if(playerInRange && !activateInteractable) // || Input.GetMouseButtonDown(0) && playerInRange && !activateInteractable
        {
            if (playOnTrigger)
            {
                activateInteractable = true;
                if (soundToPlay != null && !audioPlayed) { aud.PlaySound(soundToPlay); } // this needs to disable once played
                StartCoroutine("activateInteractableCooldown");
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                activateInteractable = true;
                if (soundToPlay != null && !audioPlayed) { aud.PlaySound(soundToPlay); } // this needs to disable once played
                anim.SetBool("isInteracting", true);
                StartCoroutine("activateInteractableCooldown");
            }

        }

        if (overrideStart)
        {
            activateInteractable = true;
            if (soundToPlay != null && !audioPlayed) { aud.PlaySound(soundToPlay); } // this needs to disable once played
            anim.SetBool("isInteracting", true);
            StartCoroutine("activateInteractableCooldown");
        }
    }

    public void ActivateInteractable()
    {
        activateInteractable = true;
        if (soundToPlay != null && !audioPlayed) { aud.PlaySound(soundToPlay); } // this needs to disable once played
        anim.SetBool("isInteracting", true);
        StartCoroutine("activateInteractableCooldown");
    }

    IEnumerator activateInteractableCooldown()
    {
        if(canRepeatAudio == false) { audioPlayed = true; }
        yield return new WaitForSeconds(timeToWait);
        anim.SetBool("isInteracting", false);
        interactableObject.GetComponent(scriptToInteractWith).BroadcastMessage(methodInsideInteractedScript);
        activateInteractable = false;
    }


}
