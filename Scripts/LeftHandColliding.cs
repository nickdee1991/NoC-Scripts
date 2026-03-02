using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandColliding : MonoBehaviour
{
    public Animator anim;
    public bool isLeftColliding;
    public SphereCollider leftCol;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInParent<Animator>();
        isLeftColliding = false;
    }

    private void OnTriggerStay(Collider leftBoxCol)
    {
        if (leftBoxCol.gameObject.CompareTag("Obstacle") || leftBoxCol.gameObject.CompareTag("Door") && isLeftColliding == false)
        {
            isLeftColliding = true;
            anim.SetBool("isLeftColliding", true);
        }
    }

    private void OnTriggerExit(Collider leftBoxCol)
    {
        if (leftBoxCol.gameObject.CompareTag("Obstacle") || leftBoxCol.gameObject.CompareTag("Door"))
        {
            isLeftColliding = false;

        }
        anim.SetBool("isLeftColliding", false);
    }
}
