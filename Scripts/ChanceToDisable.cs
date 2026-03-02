using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceToDisable : MonoBehaviour
{
    int randomNumber;
    public int randomNumberMax;


    public void Awake()
    {
        randomNumberMax = 4;

        randomNumber = Random.Range(0, randomNumberMax);

        if(randomNumber == 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
