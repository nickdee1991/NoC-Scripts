using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStagger : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine("AnimationStaggerTimer");
    }

    public IEnumerator AnimationStaggerTimer()
    {
        yield return new WaitForSeconds(Random.Range(0, 10));
        anim.enabled = true;
    }

}
