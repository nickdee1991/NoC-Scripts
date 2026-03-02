using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DebugTest : MonoBehaviour
{
    private GameManager gm;

    public string ObjectName;
    public float textFadeTime = .5f;
    public TextMeshProUGUI text;
    public string debugModeState;
    public Material debugOffMat;
    public Material debugOnMat;
    private bool trigger;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        debugModeState = gm.DEBUGMODE.ToString();
        text.text = "Debug Mode = " + debugModeState.ToString() + "\n DM disables Enemy LoS";
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E) && !trigger)
            {
                StartCoroutine("DebugTestTimer");
            }
        }
    }

    public IEnumerator DebugTestTimer()
    {
        trigger = true;


        if (gm.DEBUGMODE == false)
        {
            gm.DEBUGMODE = true;
            GetComponent<MeshRenderer>().material = debugOnMat;
        }
        else
        {
            gm.DEBUGMODE = false;
            GetComponent<MeshRenderer>().material = debugOffMat;
        }

        Debug.Log("DebugMode = " + gm.DEBUGMODE);

        yield return new WaitForSeconds(1f);
        trigger = false;
    }
}
