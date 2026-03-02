using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDisable : MonoBehaviour
{
    [SerializeField]
    private int randNumLength;

    void Start()
    {
        int randNum = Random.Range(0, randNumLength);

        if (randNum == 1)
        {
            gameObject.SetActive(false);
        }
    }
}
