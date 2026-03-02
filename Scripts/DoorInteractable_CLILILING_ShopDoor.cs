using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : MonoBehaviour
{
    [SerializeField]
    private float doorOpenedTimer;
    [SerializeField]
    private bool doorOpenedCooldown;

    private AudioSource aud;
    private AudioManager audMan;

    private void Start()
    {
        doorOpenedCooldown = false;
        audMan = FindObjectOfType<AudioManager>();
        aud = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!doorOpenedCooldown)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                //StartCoroutine(DoorOpenedTimer());
                aud.Play();
            }

            if (collision.gameObject.CompareTag("Player"))
            {
                StartCoroutine(DoorOpenedTimer());
                audMan.PlaySound("DoorOpen");
            }

            //Debug.Log("DoorOpened by" + collision.gameObject.name);
        }
    }

    IEnumerator DoorOpenedTimer()
    {
        doorOpenedCooldown = true;

        if (GetComponent<Rigidbody>().isKinematic)
        {
            audMan.PlaySound("DoorLocked");
            yield return new WaitForSeconds(doorOpenedTimer);
            doorOpenedCooldown = false;
        }
        else
        {
            yield return new WaitForSeconds(doorOpenedTimer);
            doorOpenedCooldown = false;
        }
    }
}
