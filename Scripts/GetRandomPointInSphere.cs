using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetRandomPointInSphere : MonoBehaviour
{
    [SerializeField]
    Transform[] randomEnemySpawnPoint;

    [SerializeField]
    private LayerMask layerMask;

    private SphereCollider sphereCollider;
    public Vector3 enemySpawnPoint;

    public int trapEventRadius;

    public bool playerInRange;

    // Start is called before the first frame update
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = trapEventRadius;
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }
    }

    public void GetRandomPointOnSpawn()
    {
        //int layerMask = 1 << 13; //Layer 11
        //GameObject scHolder = new GameObject();
        //scHolder.transform.position = gameObject.transform.position;
        //Vector3 scHolderPos = scHolder.transform.localPosition;
        //scHolder.name = "TrapSphereCollider";
        //SphereCollider sc = scHolder.AddComponent(typeof(SphereCollider)) as SphereCollider; //CREATE COLLIDER 
        //sc.isTrigger = true; // SET AS TRIGGER
        //sc.radius = trapEventRadius; //SET RADIUS OF TRIGGER

        Collider[] EnemyTrapEventSpawnPoints = Physics.OverlapSphere(sphereCollider.transform.position, sphereCollider.radius, layerMask);//layerMask  GET COLLIDERS WITHIN TRIGGER RADIUS - IGNORE ANYTHING NOT spawnpoint LAYER

        if (EnemyTrapEventSpawnPoints.Length > 0)
        {
            Debug.Log("add spawnpoints to bear trap");
            int destPoint = (Random.Range(0, EnemyTrapEventSpawnPoints.Length));
            GetComponentInParent<Beartrap>().enemySpawnPoint = EnemyTrapEventSpawnPoints[destPoint].transform;
            //Destroy(scHolder);
        }
    }

    public Vector3 GetRandomPointOnXAxis()
    {
        // Get sphere properties
        float radius = sphereCollider.radius;
        Vector3 center = sphereCollider.transform.TransformPoint(sphereCollider.center);

        // On the sphere's surface along the X axis, we'll have exactly two points
        // These points will be at (center.x ± radius, center.y, center.z)

        // Randomly choose between the two possible X coordinates
        float randomX = Random.value < 0.5f ?
            center.x - radius :
            center.x + radius;

        float randomZ = Random.value < 0.5f ?
        center.z - radius :
        center.z + radius;

        return new Vector3(randomX, center.y, randomZ);
    }
    public void SetSpawnPoint()
    {
        enemySpawnPoint = GetRandomPointOnXAxis();
        Debug.DrawLine(gameObject.transform.position, enemySpawnPoint.normalized);
    }
}
