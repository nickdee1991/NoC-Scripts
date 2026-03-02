using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogController : MonoBehaviour
{
    public ParticleSystem fog;

    public void Awake()
    {
        fog.Stop();   
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (fog.isPlaying)
            {
                Debug.Log("disable fog");
                fog.Stop();
            }
            if (fog.isStopped)
            {
                Debug.Log("enable fog");
                fog.Play();
            }
        }
    }

}
