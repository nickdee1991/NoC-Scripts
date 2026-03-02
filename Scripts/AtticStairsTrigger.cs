using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtticStairsTrigger : MonoBehaviour
{
    private Animator anim;
    private AudioSource aud;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();
    }

    public void OpenAttic()
    {
        anim.SetBool("open", true);
        aud.Play();
    }
}
