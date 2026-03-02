using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBoundsTrigger : MonoBehaviour
{
    [SerializeField]
    private float updateTime;
    [SerializeField]
    private Vector3 lastPlayerPosition;
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private Transform spawnPos;

    private void Start()
    {
        if(spawnPos == null)
        {
            spawnPos = GameObject.Find("PlayerSpawn").transform;
        }

        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine("UpdatePlayerPosition");
    }

    public IEnumerator UpdatePlayerPosition()
    {
        //Debug.Log("checking last known position "+ lastPlayerPosition);
        yield return new WaitForSeconds(updateTime);
        lastPlayerPosition = player.transform.position;
        StartCoroutine("UpdatePlayerPosition");
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(player))
        {
            if (spawnPos != null)
            {
                player.transform.position = spawnPos.transform.position;
            }
            else
            {
                //teleport to player last known position
                player.transform.position = lastPlayerPosition;
                StartCoroutine("UpdatePlayerPosition");
            }
        }

        if (other.gameObject.GetComponentInParent<Item>())
        {
            //store parent
            Item parentOfItem = other.gameObject.GetComponentInParent<Item>();

            //store item position
            Vector3 itemSpawnPosition = parentOfItem.defaultSpawnPoint;

            Debug.Log(other.gameObject.name + " moving to " + itemSpawnPosition);
            parentOfItem.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
            parentOfItem.transform.position = itemSpawnPosition;
        }
    }

}
