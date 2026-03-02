using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarIdle : MonoBehaviour
{
    public bool carOn;

    public bool lightsOn;

    [SerializeField]
    private float noiseRadiusTimer;
    [SerializeField]
    private float noiseRadius;

    private GameObject enemyTarget;

    private AudioSource aud;
    private ParticleSystem smoke;
    [SerializeField]
    private Light[] headlights;
    [SerializeField]
    private GameObject[] headLightObjects;


    // Start is called before the first frame update
    void Start()
    {

        aud = GetComponent<AudioSource>();
        smoke = GetComponentInChildren<ParticleSystem>();
        smoke.Stop();

        if (carOn)
        {
            aud.Play();
            smoke.Play();
            GetComponent<Animator>().enabled = true;
        }

        if (lightsOn)
        {
            foreach (Light light in headlights)
            {
               light.enabled = true;
            }
            foreach (GameObject headLightObject in headLightObjects) 
            {
                headLightObject.SetActive(true);
            }
        }
    }

    public void CarOn()
    {
        aud.Play();
        smoke.Play();
        GetComponent<Animator>().enabled = true;

        foreach (Light light in headlights)
        {
            light.enabled = true;
        }

        foreach (GameObject headLightObject in headLightObjects)
        {
            headLightObject.SetActive(true);
        }

        CarAlert();
    }

    public void CarAlert()
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
            enemyTarget.GetComponent<Enemy>().investigatingEvidence = true;
            enemyTarget.GetComponent<Enemy>().movingToEvidence = true;
            enemyTarget.GetComponent<Enemy>().InvestigateLocation = gameObject;


            Debug.Log(RandomEnemyInRange.gameObject.name + "Is investigating " + gameObject.name);
            //sc.radius = 0; // soft disable collider
            Destroy(scHolder);
        }
    }
}
