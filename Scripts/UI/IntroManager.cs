using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    public AnimatorClipInfo[] animClipInfo;

    private float introLength;

    private void Start()
    {
        animClipInfo = anim.GetCurrentAnimatorClipInfo(0);

        

        introLength = animClipInfo[0].clip.length;

        GetComponent<AudioSource>().Play();
        StartCoroutine("IntroTimer");
    }
    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.E))
        {
            LoadMenu();
            Debug.Log("Load Level " + SceneManager.GetActiveScene().buildIndex +1);
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu");
        }
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }

    IEnumerator IntroTimer()
    {
        yield return new WaitForSeconds(introLength -1);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene("1Mansion");
    }
}
