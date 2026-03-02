using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPhysicsEnable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetComponent<Rigidbody>().isKinematic = false;
            gameObject.transform.parent = null;
        }
    }
}
