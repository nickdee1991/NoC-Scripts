using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EavesdroppingTrigger : MonoBehaviour
{
    [SerializeField]
    private EnemyVoiceManager enemyVoice;

    private Enemy enem;

    private void Start()
    {
        enem = enemyVoice.GetComponentInParent<Enemy>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(enem != null)
        {
            if (other.gameObject.CompareTag("Player") && enem.idleEnemy)
            {
                //Debug.Log("player in eavesdropping trigger");
                enemyVoice.playOnTrigger = true;
            }
        }

    }
}


