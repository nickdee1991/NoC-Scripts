using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturedScene : MonoBehaviour
{
    private Animator anim;
    [SerializeField]
    private GameObject capturedSceneHolder;
    [SerializeField]
    private GameObject[] enemyToEnable;

    public int enemyThatCaptured;

    [SerializeField] 
    private Camera capturedCamera;

    private float capturedSceneTimer = 5;

    private void Start()
    {
        capturedCamera = GetComponentInChildren<Camera>();
        anim = GetComponent<Animator>();
    }

    public void CapturedSceneTrigger()
    {
        StartCoroutine("CapturedSceneStart");
    }

    public IEnumerator CapturedSceneStart()
    {
        for (int i = 0; i < enemyToEnable.Length; i++)
        {
            if (enemyThatCaptured == i)
            {
                Debug.Log(i + " is the enemy that captured you");
                enemyToEnable[i].gameObject.SetActive(true);
            }
        }

        anim.SetBool("play", true);
        capturedCamera.enabled= true;
        yield return new WaitForSeconds(capturedSceneTimer);

        foreach (GameObject enemy in enemyToEnable)
        {
            enemy.SetActive(false);
        }

        anim.SetBool("play", false);
        capturedCamera.enabled = false;

    }
}
