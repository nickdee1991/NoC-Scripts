using UnityEngine;

public class EventsTrigger : MonoBehaviour
{
    public bool playerInRange;
    public bool eventTriggered;

    [SerializeField]
    private float eventTime;

    public enum EventState { GUNSHOT, TRUCK, BEARTRAP, FATHER }
    public EventState CurrentEventState;

    [SerializeField]
    private GameObject GUNSHOT;
    [SerializeField]
    private GameObject TRUCK;
    [SerializeField]
    private GameObject BEARTRAP;
    [SerializeField]
    private GameObject FATHER;

    private void Start()
    {
        eventTriggered = false;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && eventTriggered == false)
        {
            Debug.Log("eventTriggered");

            playerInRange = true;
            eventTriggered = true;

            EventTrigger();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))// && eventTriggered == false
        {
            Debug.Log("left event range");
            playerInRange = false;
        }

    }

    public void EventTrigger()
    {
        CurrentEventState = (EventState)Random.Range(0, System.Enum.GetValues(typeof(EventState)).Length);
        //Debug.Log(gameObject.transform.parent.name + " event state is currently " + CurrentEventState);

        switch (CurrentEventState)
        {
            case EventState.GUNSHOT:
                //Distant gunshot - scattering birds
                GUNSHOT.SetActive(true);
                break;
        }
        switch (CurrentEventState)
        {
            case EventState.TRUCK:
                //Distant scream - spawned truck
                TRUCK.SetActive(true);
                break;
        }
        switch (CurrentEventState)
        {
            case EventState.BEARTRAP:
                //Distant beatrap - scream - spawned escaper
                BEARTRAP.SetActive(true);
                break;
        }
        switch (CurrentEventState)
        {
            case EventState.FATHER:
                //Distant voice - distant gunshot - spawn father
                FATHER.SetActive(true);
                break;
        }
    }
}
