using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockpickPuzzlePins : MonoBehaviour
{
    [SerializeField]
    private Transform PickDefaultPosition;

    [SerializeField]
    private GameObject Pick;

    private AudioSource aud;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.Equals(Pick))
        {
            GetComponent<Animator>().enabled = false;
            aud.Play();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.Equals(Pick))
        {
            GetComponent<Animator>().enabled = true;
            Pick.transform.position = PickDefaultPosition.position;
        }
    }
}
