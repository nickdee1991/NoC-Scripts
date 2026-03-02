using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClosetDoors : MonoBehaviour
{
    [SerializeField]
    private bool doorOpened;

    [SerializeField]
    private Animator anim;

    [SerializeField]
    private AudioSource aud;

    [SerializeField]
    private AudioManager audMan;

    public float doorOpenedTime;

    private void Start()
    {
        doorOpened = false;
        anim = GetComponent<Animator>();
        audMan = FindObjectOfType<AudioManager>();
        aud = GetComponent<AudioSource>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                FindObjectOfType<PlayerSimpleMovement>().isInteracting = true;
                if (doorOpened && anim.GetBool("open")==true)
                {
                    StartCoroutine("ClosetCloseTimer");
                }

                if (!doorOpened && anim.GetBool("open") == false)
                {
                    anim.SetBool("open", true);
                    doorOpened = true;
                    //Debug.Log("door opened = " + doorOpened);
                }
            }
        }

        if (other.gameObject.CompareTag("Enemy")&& other.gameObject.GetComponent<Enemy>().searchingForPlayer == true)
        {
            if (!doorOpened && anim.GetBool("open") == false)
            {
                anim.SetBool("open", true);
                audMan.PlaySound("DoorOpen");
                doorOpened = true;
                //Debug.Log("door opened = " + doorOpened);
            }
        }
    }

    public IEnumerator ClosetCloseTimer()
    {
        aud.Play();
        yield return new WaitForSeconds(doorOpenedTime);
        anim.SetBool("open", false);
        doorOpened = false;
        //Debug.Log("door opened = " + doorOpened);
    }
}
