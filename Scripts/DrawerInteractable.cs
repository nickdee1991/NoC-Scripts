using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerInteractable : MonoBehaviour
{

    private Animator anim;
    private AudioSource aud;
    private float openCooldown = 0.5f;
    private bool drawerCooldown;

    void Start()
    {
        anim = GetComponentInParent<Animator>();
        drawerCooldown = false;
    }

    public void DrawerInteract()
    {
        //Debug.Log("Drawer Interact");
        if (!drawerCooldown)
        {
            drawerCooldown = true;
            Debug.Log("Drawer Opened");
            //LeanTween.moveX(gameObject, -1f, 2f);
            if (anim.GetBool("open") == true)
            {
                anim.SetBool("open", false);
            }
            else
            {
                anim.SetBool("open", true);
            }
            //aud.Play();
            StartCoroutine("DrawerCooldown");
        }
    }

    private IEnumerator DrawerCooldown()
    {
        yield return new WaitForSeconds(openCooldown);
        drawerCooldown = false;
    }
}
