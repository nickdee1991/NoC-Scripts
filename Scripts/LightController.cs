using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] lightObjects;

    [SerializeField]
    private int lightRandomRoll;

    void Awake()
    {
        lightRandomRoll = Random.Range(1, 3);
    }

    void Start()
    {

        if (lightRandomRoll == 1)
        {
            foreach (GameObject lightObject in lightObjects)
            {
                lightObject.SetActive(false);
            }
        }

        if (lightRandomRoll == 2)
        {
            foreach (GameObject lightObject in lightObjects)
            {
                lightObject.SetActive(true);
            }
        }
        if (lightRandomRoll == 3)
        {
            foreach (GameObject lightObject in lightObjects)
            {
                lightObject.SetActive(true);
            }
        }
    }
}
