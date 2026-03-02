using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorboardEvidence : MonoBehaviour
{
    public bool isTriggered;

    private void Start()
    {
        isTriggered = false;
    }

    private void OnCollisionEnter(Collision collision)
    {// if player collides - and is not creeping - and hasnt already triggered trap
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<PlayerSimpleMovement>().isCreeping == false && isTriggered == false)
        {
            GetComponent<AudioSource>().Play();
            GetComponent<SphereCollider>().enabled = true;
            isTriggered = true;
            StartCoroutine("NoiseRadiusTimer");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isTriggered == true)
        {
            isTriggered = false;
            GetComponent<SphereCollider>().enabled = false;
        }
    }

    IEnumerator NoiseRadiusTimer() // disable noise radius after set time
    {
        yield return new WaitForSeconds(1);
        GetComponent<SphereCollider>().enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && other.GetComponent<Enemy>().detectedEnemy == false)
        {
            Debug.Log(other.gameObject.name + "heard a noise");
            other.GetComponent<Enemy>().investigatingEvidence = true;
            other.GetComponent<Enemy>().movingToEvidence = true;
            other.gameObject.GetComponent<Enemy>().InvestigateLocation = gameObject;
            other.gameObject.BroadcastMessage("Investigate");
;        }
        //FindObjectOfType<Enemy>().BroadcastMessage("Attack"); // will not alert multiple enemies currently
    }

}
