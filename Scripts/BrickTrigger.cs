using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickTrigger : MonoBehaviour
{
    [SerializeField]
    private float brickMovement;
    [SerializeField]
    //private bool brickFall;
    private bool brickHasFallen;

    private void Start()
    {
        //brickFall = false;
        brickMovement = 0;
    }

    private void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Brick selected");
            transform.Translate(0,0,0.05f, Space.Self);
            brickMovement ++;
            if(brickMovement >= 5 && brickHasFallen == false)
            {
                //brickFall = true;
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<Rigidbody>().isKinematic = false;
                brickHasFallen = true;
            }
        }
    }
}
