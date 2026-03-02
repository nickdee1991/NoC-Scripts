using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCollision : MonoBehaviour
{
    [SerializeField]
    private GameObject BreakageParticle;
    [SerializeField]
    private GameObject ImpactParticle;

    [SerializeField]
    private bool spawnItemOnBreak; //i just made this so you could hold the brain

    [SerializeField]
    private GameObject itemToSpawn; //lets be honest this should just be called brainToSpawn

    public GameObject enemyTarget;
    [SerializeField]
    private float noiseRadius;

    public bool isBroken; // trigger for enemy noticing object

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !isBroken) // || collision.gameObject.CompareTag("Obstacle") && !isBroken
        {
            GetComponent<AudioSource>().Play();
            if (GetComponent<SphereCollider>())
            {
                GetComponent<SphereCollider>().enabled = true;
            }

            BreakageParticle.GetComponent<ParticleSystem>().Play();
            ImpactParticle.GetComponent<ParticleSystem>().Play();
            isBroken = true;

            if (GetComponent<Item>().enabled == true)
            {
                GetComponent<Item>().itemDestroyed = true;
                GetComponent<Item>().ObjectName = "Broken " + GetComponent<Item>().ObjectName;
                if (spawnItemOnBreak)
                {
                    itemToSpawn.transform.position = transform.position;//new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.25f, gameObject.transform.position.z - 0.5f);//Instantiate(itemToSpawn, transform);
                    StartCoroutine("BrokenItemDestroyTimer");

                }
            }

            StartCoroutine("NoiseRadiusTimer");
        }
    }

    IEnumerator BrokenItemDestroyTimer()
    {
        yield return new WaitForSeconds(1f);
        itemToSpawn.transform.parent = null;
        itemToSpawn.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        gameObject.SetActive(false);

    }

    IEnumerator NoiseRadiusTimer() // disable noise radius after set time
    {
        //alert nearby enemies
        int layerMask = 1 << 6; //Layer 11
        GameObject scHolder = new GameObject();
        scHolder.transform.position = gameObject.transform.position;
        Vector3 scHolderPos = scHolder.transform.localPosition;
        scHolder.name = "TrapSphereCollider";
        SphereCollider sc = scHolder.AddComponent(typeof(SphereCollider)) as SphereCollider; //CREATE COLLIDER 
        sc.isTrigger = true; // SET AS TRIGGER
        sc.radius = noiseRadius; //SET RADIUS OF TRIGGER - FORGIVE HARDCODE PLS

        Collider[] EnemyColliders = Physics.OverlapSphere(scHolderPos, sc.radius, layerMask);//GET COLLIDERS WITHIN TRIGGER RADIUS - IGNORE ANYTHING NOT HIDINGSPOT LAYER

        if (EnemyColliders.Length > 0)
        {
            Collider RandomEnemyInRange;

            Collider bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            foreach (Collider EnemyInRange in EnemyColliders)
            {
                Vector3 directionToTarget = EnemyInRange.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = EnemyInRange;
                }
            }
            RandomEnemyInRange = bestTarget;

            enemyTarget = RandomEnemyInRange.gameObject;
            if (enemyTarget.GetComponent<Enemy>().startIdleEnemy == false)
            {
                enemyTarget.GetComponent<Enemy>().startIdleEnemy = true;
            }

            if (enemyTarget.GetComponent<Enemy>().detectedEnemy == false)
            {
                enemyTarget.GetComponent<Enemy>().investigatingEvidence = true;
                enemyTarget.GetComponent<Enemy>().movingToEvidence = true;
                enemyTarget.GetComponent<Enemy>().InvestigateLocation = gameObject;
            }


                Debug.Log(RandomEnemyInRange.gameObject.name + "Is investigating " + gameObject.name);
            //sc.radius = 0; // soft disable collider
            Destroy(scHolder);

        }
        yield return new WaitForSeconds(1f);
    }
}
