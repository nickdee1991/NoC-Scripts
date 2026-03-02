using System.Collections;
using UnityEngine;

public class InteractableProximity : MonoBehaviour
{
    [SerializeField]
    private GameObject interactableObject; // object that is attached to this switch

    [SerializeField]
    private GameObject Player;

    [SerializeField]
    private string scriptToInteractWith; // type what script the player will interact with.

    [SerializeField]
    private string methodInsideInteractedScript; // the name for the method you want to activate in the object

    [SerializeField]
    private string soundToPlay; // sound to play
    [HideInInspector]
    public bool activateInteractable; // the switch that is activated by the player

    [SerializeField]
    private float timeToWait; // for coroutine timer - would need to be changeable to account for different interactables

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(Player) && !activateInteractable || other.gameObject.CompareTag("Player") && !activateInteractable) // || Input.GetMouseButtonDown(0) && playerInRange && !activateInteractable
        {
            activateInteractable = true;
            StartCoroutine("activateInteractableCooldown");
        }
    }

    IEnumerator activateInteractableCooldown()
    {
        interactableObject.GetComponent(scriptToInteractWith).BroadcastMessage(methodInsideInteractedScript);
        yield return new WaitForSeconds(timeToWait);
        activateInteractable = false;
    }
}
