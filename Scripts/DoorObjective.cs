using System.Collections;using System.Collections.Generic;
using UnityEngine;

public class DoorObjective : MonoBehaviour
{
    public ObjectiveParent objPar;
    public float doorPush;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && objPar.ObjectivesComplete >= objPar.ObjectivesToComplete)
        {
            DoorUnlock();
        }
    }

    public void DoorUnlock()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().AddForce(transform.forward * 500);
    }
}
