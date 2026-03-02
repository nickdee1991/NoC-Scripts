using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNoiseRange : MonoBehaviour
{
    [SerializeField]
    private SphereCollider noiseRange;

    [SerializeField]
    private GameObject Player;

    public void Start()
    {
        noiseRange = GetComponent<SphereCollider>();
    }
    public void EnableNoiseRange()
    {
        if (noiseRange.enabled == false) { noiseRange.enabled = true; }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (other.gameObject.GetComponent<Enemy>().detectedEnemy == false) //&& GetComponent<Enemy>().investigatingEvidence == false
            {
                if (other.gameObject.GetComponent<Enemy>().startIdleEnemy == false && other.gameObject.GetComponent<Enemy>().idleEnemy == true)
                {
                    other.gameObject.GetComponent<Enemy>().startIdleEnemy = true; // THIS SHOULD BE CHANGED TO THE NEW SCRIPT
                }

                //Debug.Log(other.gameObject.name + "heard a noise");
                other.gameObject.GetComponent<PatrolRandom>().enabled = false;
                other.gameObject.GetComponent<PatrolRandom>().agent.destination = Player.transform.position;
                other.gameObject.GetComponent<Enemy>().CurrentEnemyState = Enemy.EnemyState.INVESTIGATING;
                //This works but need to have the enemy search the general area of lastPlayerPosition rather than moving directly to Player.transform.position
                // with multiple enemies investigating it doesnt look convincing
            }
        }
    }
}
