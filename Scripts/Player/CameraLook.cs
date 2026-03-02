using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class CameraLook : MonoBehaviour
{
    public Transform player;
    [SerializeField]
    private Camera PlayerCamera;
    public float smooth;

    public float mouseSensitivity;

    float xAxisClamp = 0.0f;

    private Vector3 velocity = Vector3.zero;

    private RaycastHit rayText;
    public TextMeshProUGUI text;
    [SerializeField]
    private float raylength;
    [SerializeField]
    private float RangeCooldownTimer;
    [SerializeField]
    public GameObject itemInRange;

    private UIManager UIManager;
    [SerializeField]
    private bool unlockCursor; // integrate later, for menus
    [SerializeField]
    private bool rangeCooldown;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        UIManager = FindObjectOfType<UIManager>();
        text = UIManager.objectText;
        PlayerCamera = GetComponent<Camera>();
    }


    private void Update()
    {
        RotateCamera();

        if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out rayText, raylength))
        {
            if(rangeCooldown == false)
            {
                Debug.DrawRay(PlayerCamera.transform.position, PlayerCamera.transform.forward * raylength, Color.red, 2f);

                PlayerInRangeOfItem();
            }
        }
    }

    void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float rotAmountX = mouseX * mouseSensitivity;
        float rotAmountY = mouseY * mouseSensitivity;

        xAxisClamp -= rotAmountY;

        Vector3 targetRotCam = transform.rotation.eulerAngles;
        Vector3 targetRotBody = player.rotation.eulerAngles;

        targetRotCam.x -= rotAmountY;
        targetRotCam.z = 0;
        targetRotBody.y += rotAmountX;

        if (xAxisClamp > 90)
        {
            xAxisClamp = 90;
            targetRotCam.x = 90;
        }
        else if (xAxisClamp < -90)
        {
            xAxisClamp = -90;
            targetRotCam.x = 270;
        }


        transform.rotation = Quaternion.Euler(targetRotCam);
        player.rotation = Quaternion.Euler(targetRotBody);
    }

    void PlayerInRangeOfItem()
    {
        if (rayText.transform.GetComponent<Item>())
        {
            itemInRange = rayText.transform.gameObject;
            text.text = itemInRange.GetComponent<Item>().ObjectName;
            itemInRange.GetComponent<Item>().playerInRange = true;
            UIManager.UIObjectEnable();
            UIManager.UICrossHairSet();
            StartCoroutine("RangeCooldown");
            //Debug.Log("player linecast has hit " + itemInRange.GetComponent<Item>().ObjectName);
        }
        if (rayText.transform.GetComponent<ObjectiveParent>())
        {
            itemInRange = rayText.transform.gameObject;
            text.text = itemInRange.GetComponent<ObjectiveParent>().ObjectName;
            itemInRange.GetComponent<ObjectiveParent>().playerInRange = true;
            UIManager.UIObjectEnable();
            UIManager.UICrossHairSet();
            StartCoroutine("RangeCooldown");
            //Debug.Log("player linecast has hit " + itemInRange.GetComponent<ObjectiveParent>().ObjectiveParentName);
        }

        if (rayText.transform.GetComponent<Interactable>())
        {
            //string interactableName = itemInRange.GetComponent<Interactable>().ObjectName;

            itemInRange = rayText.transform.gameObject;

            if (!string.IsNullOrWhiteSpace(itemInRange.GetComponent<Interactable>().ObjectName))
            {
                text.text = itemInRange.GetComponent<Interactable>().ObjectName;
            }

            itemInRange.GetComponent<Interactable>().playerInRange = true;
            UIManager.UICrossHairSet();
            UIManager.UIObjectEnable();
            StartCoroutine("RangeCooldown");
            //Debug.Log("player linecast has hit " + itemInRange.GetComponent<Interactable>().ObjectName);
        }

        if (rayText.transform.GetComponentInChildren<HidingSpot>())
        {
            itemInRange = rayText.transform.gameObject;
            text.text = itemInRange.GetComponentInChildren<HidingSpot>().ObjectName;
            itemInRange.GetComponentInChildren<HidingSpot>().playerInRange = true;
            UIManager.UICrossHairSet();
            UIManager.UIObjectEnable();
            StartCoroutine("RangeCooldown");
            //Debug.Log("player linecast has hit " + itemInRange.GetComponentInChildren<HidingSpot>().ObjectName);
        }
    }

    IEnumerator RangeCooldown()
    {
        rangeCooldown = true;
        yield return new WaitForSeconds(RangeCooldownTimer);
        UIManager.UIObjectDisable();
        UIManager.UICrossHairUnset();

        if (itemInRange.transform.GetComponent<Item>() && itemInRange.GetComponent<Item>().playerInRange == true)//  && !rayText.transform.GetComponent<Item>()
        {
            itemInRange.GetComponent<Item>().playerInRange = false;

        }

        if (itemInRange.transform.GetComponent<ObjectiveParent>() && itemInRange.GetComponent<ObjectiveParent>().playerInRange == true)
        {
            itemInRange.GetComponent<ObjectiveParent>().playerInRange = false;
        }

        if (itemInRange.transform.GetComponent<Interactable>() && itemInRange.GetComponent<Interactable>().playerInRange == true)
        {
            itemInRange.GetComponent<Interactable>().playerInRange = false;
        }

        if (itemInRange.transform.GetComponentInChildren<HidingSpot>())
        {
            itemInRange.GetComponentInChildren<HidingSpot>().playerInRange = false;
        }

        if (itemInRange.transform.GetComponentInChildren<PuzzleInteractable>())
        {
            itemInRange.GetComponentInChildren<PuzzleInteractable>().playerInRange = false;
        }

        rangeCooldown = false;
    }
}
