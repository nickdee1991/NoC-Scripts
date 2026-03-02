using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrap : MonoBehaviour
{
    [SerializeField]
    private int noiseRadius = 50;

    private Rigidbody rb;
    [SerializeField]
    private bool attractenemy;
    //create collision to enemys head
    //find best route to register collision with head
    //measure rigidbody speed - so enemy cant register if they just touch it
    //make sure player can push this

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        //Debug.Log("velocity.magnitude = "+rb.velocity.magnitude);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (rb.velocity.magnitude > 5)
            {
                Enemy enemyHit = collision.gameObject.GetComponent<Enemy>();
                enemyHit.CurrentTrappedState = Enemy.TrappedState.UNCONSIOUS;
                Debug.Log("hit enemy");
            }


            #region // enemy sound radius
            int layerMask = 1 << 6; //Layer 11
            GameObject scHolder = new GameObject();
            scHolder.transform.position = gameObject.transform.position;
            Vector3 scHolderPos = scHolder.transform.localPosition;
            scHolder.name = "TrapSphereCollider";
            SphereCollider sc = scHolder.AddComponent(typeof(SphereCollider)) as SphereCollider; //CREATE COLLIDER 
            sc.isTrigger = true; // SET AS TRIGGER
            sc.radius = noiseRadius; //SET RADIUS OF TRIGGER

            Collider[] EnemyColliders = Physics.OverlapSphere(scHolderPos, sc.radius, layerMask);//GET COLLIDERS WITHIN TRIGGER RADIUS - IGNORE ANYTHING NOT HIDINGSPOT LAYER

            if (EnemyColliders.Length > 0 && attractenemy)
            {
                int RandomEnemy = Random.Range(0, EnemyColliders.Length); // ROLL FOR RANDOM ENEMY IN THE ARRAY

                Enemy RandomEnemyToInvestigate = EnemyColliders[RandomEnemy].GetComponent<Enemy>(); // GET THE ENEMY SCRIPT IN GAME WORLD OF RANDOM ENEMY

                //RandomEnemyToInvestigate.BroadcastMessage("Investigate"); // this was set to ATtack, why?
                if (RandomEnemyToInvestigate.startIdleEnemy == false)
                {
                    RandomEnemyToInvestigate.startIdleEnemy = true;
                }

                if (RandomEnemyToInvestigate.detectedEnemy == false)
                {
                    RandomEnemyToInvestigate.investigatingEvidence = true;
                    RandomEnemyToInvestigate.movingToEvidence = true;
                    RandomEnemyToInvestigate.InvestigateLocation = gameObject;
                }
                Debug.Log(RandomEnemyToInvestigate.gameObject.name + "Is investigating " + gameObject.name);
                //sc.radius = 0; // soft disable collider
                Destroy(scHolder);
            }
                #endregion 
            }
    }
}
