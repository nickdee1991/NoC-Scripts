using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpotTest : MonoBehaviour
{
    public LayerMask laymask;
    public Collider[] hsCol;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            HidingSpotCheck();
            Debug.Log("pressing B");
        }
    }

    public void HidingSpotCheck()
    {
        GameObject scHolder = new GameObject();
        scHolder.name = "HidingSpotSphereCollider";
        scHolder.transform.parent = gameObject.transform;
        //scHolder.transform.parent = gameObject.transform.parent;
        scHolder.transform.position = gameObject.transform.position;
        scHolder.layer = 12;
        Vector3 scHolderPos = scHolder.transform.localPosition;
        SphereCollider sc = scHolder.AddComponent(typeof(SphereCollider)) as SphereCollider; //CREATE COLLIDER 
        //Physics.IgnoreCollision(sc, GetComponent<SphereCollider>());
        Physics.IgnoreLayerCollision(12, 0);
        int layerMask = 1 << 11; //Layer 11

        sc.isTrigger = true; // SET AS TRIGGER
        sc.radius = 20; //SET RADIUS OF TRIGGER - FORGIVE HARDCODE PLS

        Collider[] hidingSpotColliders = Physics.OverlapSphere(scHolderPos, sc.radius, layerMask);//GET COLLIDERS WITHIN TRIGGER RADIUS - IGNORE ANYTHING NOT HIDINGSPOT LAYER
        hsCol = hidingSpotColliders;

        if (hidingSpotColliders.Length > 0)
        {
            //hidingSpotColliders = hidingSpotColliderss;
            int HidingSpots = Random.Range(0, hidingSpotColliders.Length); // ROLL FOR RANDOM SPOT IN THE ARRAY

            Transform HidingSpotToCheck = hidingSpotColliders[HidingSpots].transform; // GET THE POSITION IN GAME WORLD OF RANDOM SPOT

            Debug.Log("Hiding spots in range " + hidingSpotColliders.Length); // COUNT AMOUNT OF HIDING SPOTS IN RANGE
            Debug.Log("Hiding spot to check " + HidingSpotToCheck.name); // PRINT NAME OF CHOSEN HIDING SPOT

            //sc.radius = 0; // soft disable collider
            Destroy(scHolder);
        }
        else
        {
            Debug.Log("Hiding spots in range " + hidingSpotColliders.Length);
        }
    }
}
