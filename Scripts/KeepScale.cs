using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepScale : MonoBehaviour
{
    [SerializeField]
    private GameObject[] objectsToKeepScale;

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject objects in objectsToKeepScale)
        {
            transform.localScale = transform.localScale;
        }
    }
}
