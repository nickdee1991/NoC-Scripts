using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickTriggerObjective : MonoBehaviour
{
    private AudioManager aud;
    [SerializeField]
    private AudioSource brickMoveAud;
    [SerializeField]
    private AudioSource brickReleaseAud;
    [SerializeField]
    private float brickMovement;
    [SerializeField]
    //private bool brickFall;
    private bool brickHasFallen;

    [SerializeField]
    private Item objectiveItem;

    private void Start()
    {
        brickMovement = 0;
        aud = FindObjectOfType<AudioManager>();
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Brick selected");
            transform.Translate(0,0,0.05f, Space.Self);
            brickMovement ++;
            brickMoveAud.Play();
            if (brickMovement >= 4 && brickHasFallen == false)
            {
                brickMoveAud.Stop();
                brickReleaseAud.Play();
                aud.PlaySound("Violin");
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<Rigidbody>().isKinematic = false;
                brickHasFallen = true;
            }
        }
    }
}
