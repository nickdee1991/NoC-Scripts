using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoInteract : MonoBehaviour
{
    private AudioSource aud;
    private float pianoInteractCooldown = 0.5f;
    private bool pianoKeyCooldown;

    public bool isTriggered;
    [SerializeField]
    private float noiseRadiusTimer;
    [SerializeField]
    private float noiseRadius;
    public GameObject enemyTarget;

    void Start()
    {
        aud = GetComponent<AudioSource>();
        pianoKeyCooldown = false;
    }

    public void PianoKeyInteract()
    {
        //Debug.Log("Drawer Interact");
        if (!pianoKeyCooldown)
        {
            pianoKeyCooldown = true;
            aud.pitch = Random.Range(0.8f,1);
            aud.Play();

            StartCoroutine("DrawerCooldown");

            #region pianoSoundRadius
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

                    Debug.Log(RandomEnemyInRange.gameObject.name + "Is investigating " + gameObject.name);
                    //sc.radius = 0; // soft disable collider
                    Destroy(scHolder);
                }

            }
            isTriggered = true;
            #endregion
        }


    }



    private IEnumerator DrawerCooldown()
    {
        yield return new WaitForSeconds(pianoInteractCooldown);
        pianoKeyCooldown = false;
    }
}
