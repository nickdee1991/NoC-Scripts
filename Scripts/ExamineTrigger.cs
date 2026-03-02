using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using TMPro;

public class ExamineTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject activeSpawnedCamera;
    private GameObject activeCamera;
    private GameObject player;
    private AudioManager aud;
    private UIManager UImanager;
    private TextMeshProUGUI text;
    [SerializeField]
    private string examineText;

    [SerializeField]
    private UnityEngine.Video.VideoPlayer onMedia;

    [SerializeField]
    private UnityEngine.Video.VideoPlayer offMedia;
    [SerializeField]
    private AudioSource sound;

    [SerializeField]
    private int mediaTimer;

    private bool cutsceneCooldown;

    public bool cutsceneActivated; // this is on by default. but can be used as a switch to activate this cutscene

    public bool cutscenePlaying; // this is used by other scripts to tell if the play is in this cutscene (eg to spawn an enemy nearby)

    private bool trigger;

    [SerializeField]
    private float examineTimer;

    [SerializeField]
    private string soundToPlay;
    [SerializeField]
    private string cutsceneText;

    private void Start()
    {
        cutsceneCooldown = false;
        aud = FindObjectOfType<AudioManager>();
        UImanager = FindObjectOfType<UIManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        text = UImanager.objectText;
        if (offMedia != null)
        {
            offMedia.Play();
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !trigger)
        {
            if (!cutscenePlaying)
            {
                UImanager.UIObjectEnable();
                text.text = examineText;
            }


            if (Input.GetKeyDown(KeyCode.E))
            {
                UImanager.UIObjectDisable();
                text.text = "";
                Debug.Log("ExamineTrigger Triggered");
                if (!cutscenePlaying)
                {
                    trigger = true;
                    PlayCutscene();
                }
                else
                {
                    trigger = true;
                    StartCoroutine("CancelTriggerTimer");
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            UImanager.UIObjectDisable();
            text.text = "";
        }
    }

    void PlayCutscene()
    {
        if (cutsceneCooldown == false && cutsceneActivated)
        {

            cutsceneCooldown = true;
            cutscenePlaying = true;

            UImanager.UIObjectDisable();
            text.text = "";
            //player.GetComponentInChildren<Animator>().SetBool("isRunning", false);
            //player.GetComponentInChildren<Animator>().SetBool("isWalking", false);
            //player.GetComponentInParent<PlayerSimpleMovement>().enabled = false;

            StartCoroutine("ExamineTriggerTimer");
        }
    }

    public IEnumerator CancelTriggerTimer()
    {
        yield return new WaitForSeconds(0.15f);
        Destroy(activeCamera);
        StopCoroutine("ExamineTriggerTimer");

        trigger = false;
        UImanager.UIObjectDisable();
        text.text = "";

        if (onMedia != null && !onMedia.isPlaying)
        {
            offMedia.Play();
        }

        //activeCamera.enabled = false;
        cutscenePlaying = false;
        cutsceneCooldown = false;
        //player.GetComponentInParent<PlayerSimpleMovement>().enabled = true;
        Debug.Log("cancelling cutscene");
    }

    public IEnumerator ExamineTriggerTimer()
    {
        yield return new WaitForSeconds(0.15f);
        trigger = false;

        if (onMedia != null && !onMedia.isPlaying)
        {
            offMedia.Stop();
            onMedia.Play();
        }
        if (sound != null && !sound.isPlaying)
        {
            sound.Play();
        }

        //activeCamera = Instantiate(activeSpawnedCamera);
        //activeSpawnedCamera.transform.parent = transform;
        //activeSpawnedCamera.transform.position = transform.position;
        //activeSpawnedCamera.transform.rotation = transform.rotation;
        StartCoroutine("ExamineTextTimer");
        yield return new WaitForSeconds(examineTimer);
        StartCoroutine("CheckTVPlayingLoop");
    }

    public IEnumerator ExamineTextTimer()
    {
        UImanager.UIObjectEnable();
        text.text = cutsceneText;
        yield return new WaitForSeconds(2f);
        UImanager.UIObjectDisable();
        text.text = "";
    }

        public IEnumerator CheckTVPlayingLoop()
    {
        Debug.Log("checking TV is playing");

        yield return new WaitForSeconds(mediaTimer);

        if (onMedia != null && !onMedia.isPlaying)
        {
            offMedia.Play();
            onMedia.Stop();
            sound.Stop();

        }
        StartCoroutine("CheckTVPlayingLoop");
    }
}
