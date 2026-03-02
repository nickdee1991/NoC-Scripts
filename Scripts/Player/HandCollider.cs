using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCollider : MonoBehaviour
{

    public Animator anim;
    private bool isColliding;
    public LeftHandColliding lhc;
    public RightHandColliding rhc;
    public BoxCollider bothHandBoxCol;
    private PlayerSimpleMovement playerSimpleMovement;

    private void Start()
    {
        playerSimpleMovement = FindObjectOfType<PlayerSimpleMovement>();
        anim = GetComponentInParent<Animator>();
        isColliding = false;
        Physics.IgnoreLayerCollision(7, 12);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag != "Door")
        {
            Physics.IgnoreCollision(bothHandBoxCol, collision.collider);
            Debug.Log(Physics.GetIgnoreCollision(bothHandBoxCol, collision.collider));           
        }
    }

    private void FixedUpdate()
    {
        if (rhc.isRightColliding || lhc.isLeftColliding)
        {
            if (rhc.isRightColliding & !lhc.isLeftColliding)
            {
                anim.SetBool("isRightColliding", true);
            }

            if (lhc.isLeftColliding & !rhc.isRightColliding)
            {
                anim.SetBool("isLeftColliding", true);
            }

            if (rhc.isRightColliding & lhc.isLeftColliding)
            {
                isColliding = true;
                //anim.SetBool("isLeftColliding", false);
                //anim.SetBool("isRightColliding", false);
                anim.SetBool("isColliding", true);
                playerSimpleMovement.isCollidingWithObstacle = true;
            }

        }
        else
        {
            anim.SetBool("isColliding", false);
            anim.SetBool("isLeftColliding", false);
            anim.SetBool("isRightColliding", false);
            playerSimpleMovement.isCollidingWithObstacle = false;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle") || other.gameObject.CompareTag("Door") && isColliding == false)
        {


        }
    }

    private void OnTriggerExit(Collider other)
    {
        isColliding = false;
        if (other.gameObject.CompareTag("Obstacle") || other.gameObject.CompareTag("Door"))
        {         
            anim.SetBool("isColliding", false);
            anim.SetBool("isLeftColliding", false);
            anim.SetBool("isRightColliding", false);
            playerSimpleMovement.isCollidingWithObstacle = false;
        }

        anim.SetBool("isColliding", false);
        anim.SetBool("isLeftColliding", false);
        anim.SetBool("isRightColliding", false);
    }
}
