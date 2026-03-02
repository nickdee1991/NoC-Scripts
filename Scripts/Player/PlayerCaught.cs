using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerCaught : MonoBehaviour
{
    private GameManager GM;
    public bool Captured;

    private float capturedCooldown;
    public GameObject enemyCapturedBy;
    private AudioManager aud;

    // Start is called before the first frame update
    void Start()
    {
        capturedCooldown = 3.5f;
        GM = FindObjectOfType<GameManager>();
        aud = FindObjectOfType<AudioManager>();
        Captured = false;
    }

    private void OnCollisionEnter(Collision collision) //this capture is for just touching the enemy
    {
        if (collision.gameObject.CompareTag("Enemy") && Captured == false)
        {
            enemyCapturedBy = collision.gameObject;
            enemyCapturedBy.GetComponent<Enemy>().detectedEnemy = false;
            enemyCapturedBy.GetComponentInParent<NavMeshAgent>().speed = 0;
            enemyCapturedBy.GetComponentInParent<NavMeshAgent>().ResetPath();
            GM.enemyThatCapturedPlayer = enemyCapturedBy;
            StartCoroutine("EnemyColliderCooldown");
            GM.Captured();
            Captured = true;
        }
    }

    public void RangedCapture() // this is based off them chasing you and going < enemyRange
    {
        enemyCapturedBy.GetComponent<Enemy>().detectedEnemy = false;
        enemyCapturedBy.GetComponentInParent<NavMeshAgent>().speed = 0;
        enemyCapturedBy.GetComponentInParent<NavMeshAgent>().ResetPath();
        GM.enemyThatCapturedPlayer = enemyCapturedBy;
        StartCoroutine("EnemyColliderCooldown");
        GM.Captured();
        Captured = true;
    }

    IEnumerator EnemyColliderCooldown()
    {
        yield return new WaitForSeconds(capturedCooldown);
        enemyCapturedBy.GetComponent<BoxCollider>().enabled = true;
        Captured = false;
    }
}
