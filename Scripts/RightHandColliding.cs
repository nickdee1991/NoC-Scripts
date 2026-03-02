using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHandColliding : MonoBehaviour
{
    public Animator anim;
    public bool isRightColliding;
    public SphereCollider rightCol;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInParent<Animator>();
        isRightColliding = false;
    }

    private void OnTriggerStay(Collider rightBoxCol)
    {
        if (rightBoxCol.gameObject.CompareTag("Obstacle") || rightBoxCol.gameObject.CompareTag("Door") && isRightColliding == false)
        {
            isRightColliding = true;
            anim.SetBool("isRightColliding", true);
        }
    }

    private void OnTriggerExit(Collider rightBoxCol)
    {
        if (rightBoxCol.gameObject.CompareTag("Obstacle") || rightBoxCol.gameObject.CompareTag("Door"))
        {
            isRightColliding = false;

        }
        anim.SetBool("isRightColliding", false);
    }
}
