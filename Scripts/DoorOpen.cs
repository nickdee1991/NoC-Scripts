using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();

    }

    public void CarDoorOpen()
    {
        if(anim.GetBool("open") == true)
        {
            anim.SetBool("open", false);
        }
        else
        {
            anim.SetBool("open", true);
        }
    }
}
