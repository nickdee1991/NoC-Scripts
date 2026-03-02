using System.Collections;
using UnityEngine;

public class AlarmEvidence : MonoBehaviour
{
    public bool isTriggered;
    [SerializeField]
    private float noiseRadiusTimer;
    [SerializeField]
    private float noiseRadius;
    public GameObject enemyTarget;
    [SerializeField]
    private Animator[] alarm; // trigger alarms when player interacts

    private void Start()
    {
        isTriggered = false;
    }

    private void OnTriggerEnter(Collider other)
    {// if player collides - and is not creeping - and hasnt already triggered trap
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PlayerSimpleMovement>().isCreeping == false && isTriggered == false)
        {
            GetComponent<AudioSource>().Play();

            foreach(Animator alarmAnim in alarm)
            {
                alarmAnim.SetBool("activated", true);
            Debug.Log("activated alarm" + alarmAnim.GetComponent<Animator>());
            }


            //alert nearby enemies
            int layerMask = 1 << 6; //Layer 11
            GameObject scHolder = new GameObject();
            scHolder.transform.position = gameObject.transform.position;
            Vector3 scHolderPos = scHolder.transform.localPosition;
            scHolder.name = "TrapSphereCollider";
            SphereCollider sc = scHolder.AddComponent(typeof(SphereCollider)) as SphereCollider; //CREATE COLLIDER 
            sc.isTrigger = true; // SET AS TRIGGER
            sc.radius = noiseRadius; //SET RADIUS OF TRIGGER

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
                    Destroy(scHolder);
                }

            }
            isTriggered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && isTriggered == true)
        {
            StartCoroutine("NoiseRadiusTimer");
            foreach (Animator alarmAnim in alarm)
            {
                alarmAnim.SetBool("activated", false);
                Debug.Log("deactivated alarm" + alarmAnim.GetComponent<Animator>());
            }
            GetComponent<AudioSource>().Stop();
        }
    }


    IEnumerator NoiseRadiusTimer() // disable noise radius after set time
    {
        yield return new WaitForSeconds(noiseRadiusTimer);
        isTriggered = false;
    }
}
