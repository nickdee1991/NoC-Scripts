using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

[System.Serializable]
public class CutsceneTest
{
    private CutsceneTrigger source;
    [Header("Cutscene Text")]
    #region
    public string textName;
    //public float startTextDelay;
    public float textDelay = 0.1f;
    //public float endTextDelay;

    [TextArea(3, 10)]
    public string cutSceneText;
    #endregion
       
    public void UseCutsceneText(CutsceneTrigger _source)
    {
        source = _source;
        _source.textToUse.text = cutSceneText;
    }
}

public class CutsceneTrigger : MonoBehaviour
{
    //---------------------------------------------------------//
    [Header("Cutscene Camera Setup")]
    [Header("----------------------------------------------")]
    [SerializeField]
    public Camera[] cutsceneCameras;

    public static CutsceneTrigger instance;

    [SerializeField]
    public CutsceneTest[] cutTest;

    [SerializeField]
    private Animator cutsceneStart;

    [SerializeField]
    private Camera activeCamera;

    public GameObject player;

    private AudioManager aud;

    private UIManager UImanager;

    [SerializeField]
    private AudioSource audSource;
    [SerializeField]
    private TextMeshProUGUI text;

    [SerializeField]
    private float CutsceneTimer;
    [SerializeField]
    private int cutsceneCamera;
    [SerializeField]
    private int cameraNumber;

    private bool cutsceneCooldown;

    [Header("Button Settings")]
    [Header("----------------------------------------------")]

    public bool cutsceneActivated; // this is on by default. but can be used as a switch to activate this cutscene

    public bool cutscenePlaying; // this is used by other scripts to tell if the play is in this cutscene (eg to spawn an enemy nearby)

    public bool oneTimeCutscene; // will this disable after first play?

    public bool pressbuttontoactivate; // press a button once in trigger to activate - otherwise will trigger on collision

    public bool dialogueToRun; // select to enable dialogue script in camera to play

    public bool playSound; // will play a sound at start of trigger

    private bool isPlaying; // bool that needs to exist for some reason - stops double pressing key



    [SerializeField]
    private string soundToPlay;
    [SerializeField]
    private string cutsceneText;

    private string currentText = "";
    public TMPro.TextMeshProUGUI textToUse;

    private void Start()
    {
        cutsceneStart = GetComponentInChildren<Animator>();
        cutsceneCooldown = false;
        aud = FindObjectOfType<AudioManager>();
        UImanager = FindObjectOfType<UIManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        text = UImanager.objectText;//GameObject.FindGameObjectWithTag("UItext").GetComponentInChildren<TextMeshProUGUI>();
        textToUse = GameObject.Find("GameManager").GetComponentInChildren<TextMeshProUGUI>();
        isPlaying = false;
        GetComponent<MeshRenderer>().enabled = false;
        cameraNumber = 0;
        activeCamera = cutsceneCameras[cameraNumber];
        activeCamera.enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (cameraNumber != cutsceneCameras.Length)
        {
            if (other.gameObject.CompareTag("Player") && !pressbuttontoactivate && cutsceneActivated)
            {
                if (!cutscenePlaying && !isPlaying)
                {
                    isPlaying = true;
                    PlayCutscene();
                }
                player = other.gameObject;
            }
            if (other.gameObject.CompareTag("Player") && pressbuttontoactivate && cutsceneActivated)
            {
                {
                    if (!cutscenePlaying)
                    {
                        UImanager.UIObjectEnable();
                        text.text = cutsceneText;
                    }
                    else
                    {
                        UImanager.UIObjectDisable();
                        text.text = "";
                    }

                    if (Input.GetKeyDown(KeyCode.E)&&!isPlaying)
                    {
                        if (!cutscenePlaying)
                        {
                            isPlaying = true;
                            PlayCutscene();
                        }
                        else
                        {
                            Debug.Log("TODO cancel cutscene");
                            StartCoroutine("ExitCutscene");
                        }
                        player = other.gameObject;
                    }
                }
            }

        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //text.enabled = false;
            UImanager.UIObjectDisable();
            text.text = "";
        }
    }

    void PlayCutscene()
    {
        if (cutsceneCooldown == false && cutsceneActivated)
        {
            cutsceneStart.SetBool("Start", true);
            Debug.Log("Play Cutscene");
            if (dialogueToRun)
            {

                //UseText();
            }

            if (playSound)
            {
                if (string.IsNullOrWhiteSpace(soundToPlay))
                {
                    aud.PlaySound("Cutscene");
                }
                else
                {
                    aud.PlaySound(soundToPlay);

                    if (audSource!=null)
                    {
                        audSource.Play();
                    }
                }            
            }
            UImanager.UIObjectDisable();
            text.text = "";
            StartCoroutine("CutsceneTriggerTimer1");
            player.GetComponentInChildren<Animator>().SetBool("isRunning", false);
            player.GetComponentInChildren<Animator>().SetBool("isWalking", false);
            player.GetComponentInParent<PlayerSimpleMovement>().enabled = false;
        }
    }
    
    IEnumerator ShowText()
    {
        textToUse.enabled = true;

        for (int i = 0; i < cutTest.Length; i++)
        {
            float textDelay;
            string cutSceneText = cutTest[i].cutSceneText;
            textDelay = cutTest[i].textDelay;
            currentText = cutSceneText.Substring(0, i);
            textToUse.text = currentText;
            yield return new WaitForSeconds(textDelay);
        }
    }

    public void UseText()
    {
        for (int i = 0; i < cutTest.Length; i++)
        {
            Debug.Log(cutTest[i].textName);
            Debug.Log(cameraNumber);

            if (cutTest[i].textName == cameraNumber.ToString()) //cameraNumber.ToString() +1
            {
                textToUse.enabled = true;
                cutTest[i].UseCutsceneText(this);
                textToUse.text = cutTest[i].cutSceneText;
                StartCoroutine(ShowText());
                return;
            }
        }
    }

    public IEnumerator CutsceneTriggerTimer1() //        
    {
        activeCamera.enabled = true;
        //LeanTween.moveLocalX(activeCamera.gameObject, -0.1f, 30f);
        //LeanTween.moveLocalZ(activeCamera.gameObject, -0.1f, 30f);
        yield return new WaitForSeconds(.25f);
        cutsceneCooldown = true;
        cutscenePlaying = true;

        yield return new WaitForSeconds(CutsceneTimer);

        if (dialogueToRun)
        {
            //UseText();
        }

        if (oneTimeCutscene)
        {
            GetComponent<BoxCollider>().enabled = false;
        }

        activeCamera.enabled = false;
        cameraNumber++;

        if (cameraNumber != cutsceneCameras.Length)
        {
            activeCamera = cutsceneCameras[cameraNumber];
            activeCamera.enabled = true;
            StartCoroutine("CutsceneTriggerTimer1");
        }
        else
        {
            Debug.Log("cancel out of cutscene");
            yield return new WaitForSeconds(1);
            CancelCutscene();
        }
    }

    public IEnumerator ExitCutscene()
    {
        StopCoroutine("CutsceneTriggerTimer1");
        cutsceneStart.SetBool("Start", false);
        cameraNumber = 0;
        activeCamera.enabled = false;
        activeCamera = cutsceneCameras[cameraNumber];
        player.GetComponentInParent<PlayerSimpleMovement>().enabled = true;
        UImanager.UIObjectDisable();
        text.text = "";
        yield return new WaitForSeconds(.5f);
        cutsceneCooldown = false;
        cutscenePlaying = false;
    }

    public void CancelCutscene()
    {
        cutsceneStart.SetBool("Start", false);
        cameraNumber = 0;
        activeCamera.enabled = false;
        activeCamera = cutsceneCameras[cameraNumber];
        player.GetComponentInParent<PlayerSimpleMovement>().enabled = true;
        cutsceneCooldown = false;
        cutscenePlaying = false;
        isPlaying = false;

        UImanager.UIObjectDisable();
        text.text = "";
        Debug.Log("cancel out of cutscene");
    }
}



