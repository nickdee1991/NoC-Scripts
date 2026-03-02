using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;

public class PlayerSimpleMovement : MonoBehaviour
{
    private UIManager UImanager;

    public float defaultMovementSpeed;

    public float stoppedSpeed = 0;
    public float movementSpeed;
    public float creepSpeed;
    public float sprintSpeed;

    public float sprintTime;
    public float maxSprintTime;
    public float jumpVelocity;

    private Vector3 movement;
    private Vector3 directionOfPlayer;

    public int noiseRangeIdle;
    public int noiseRangeCreeping;
    public int noiseRangeWalking;
    public int noiseRangeRunning;

    public Animator playerCameraAnim;
    public Animator playerBodyAnim;

    public bool isTrapped;
    public bool checkInventory;
    public bool isInteracting;
    public bool isCreeping;
    public bool isSprinting; // this activates while sprinting but currently doesnt do anything
    public bool isCollidingWithObstacle;

    public SphereCollider noiseRange;
    private BoxCollider playerCreepingCollider;
    private Animator sprintEffect;
    private Rigidbody rb;
    private CharacterController cc;
    [SerializeField]
    private Light Torch;

    [SerializeField]
    private float fallMultiplier = 2.5f;
    [SerializeField]
    private float lowJumpMultiplier = 2f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    bool isGrounded;
    Vector3 velocity;

    private void Start()
    {
        movementSpeed = defaultMovementSpeed;
        UImanager = FindObjectOfType<UIManager>();
        playerCreepingCollider = GetComponent<BoxCollider>();
        cc = GetComponent<CharacterController>();
        cc.detectCollisions = false;
        rb = GetComponent<Rigidbody>();
        isCollidingWithObstacle = false;
        sprintEffect = GameObject.Find("SpeedEffect").GetComponent<Animator>();
    }

    public void Update()
    {
        #region new player rb.vel
        //test();

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        // Get Horizontal and Vertical Input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirectionForward = transform.forward * verticalInput;
        Vector3 moveDirectionSide = transform.right * horizontalInput;

        //find the direction
        directionOfPlayer = (moveDirectionForward + moveDirectionSide);

        //find the distance
        movement = directionOfPlayer * movementSpeed * Time.fixedDeltaTime;
        //Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //Vector3 move = transform.right * horizontalInput + transform.forward * verticalInput;



        //if (movement != Vector3.zero)
        //  transform.forward = movement;

        velocity.y += fallMultiplier * Time.deltaTime;

        cc.Move(velocity * Time.deltaTime);
        #endregion

        #region Player Movement
        if (!isTrapped && !checkInventory)
        {

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                if (Input.GetKey(KeyCode.W))
                {
                    playerCameraAnim.SetBool("isMoving", true);
                    playerBodyAnim.SetBool("isWalkingBackward", false);
                    playerBodyAnim.SetBool("isWalking", true);
                    UImanager.PlayerSoundState = UImanager.PlayerSoundStateSprites[1];
                }

                if (Input.GetKey(KeyCode.A))
                {
                    playerBodyAnim.SetBool("isWalking", true);
                    UImanager.PlayerSoundState = UImanager.PlayerSoundStateSprites[1];
                }

                if (Input.GetKey(KeyCode.D))
                {
                    playerBodyAnim.SetBool("isWalking", true);
                    UImanager.PlayerSoundState = UImanager.PlayerSoundStateSprites[1];
                }

                if (Input.GetKey(KeyCode.S))
                {
                    // transform.Translate(Vector3.back * movementSpeed * Time.deltaTime);
                    //cc.Move(distance);
                    playerCameraAnim.SetBool("isMoving", true);
                    playerBodyAnim.SetBool("isWalkingBackward", true);
                    UImanager.PlayerSoundState = UImanager.PlayerSoundStateSprites[1];
                }

            }
            else
            {
                playerCameraAnim.SetBool("isMoving", false);
                playerBodyAnim.SetBool("isWalking", false);
                playerBodyAnim.SetBool("isWalkingBackward", false);
                playerBodyAnim.SetBool("isCrouchingBackward", false);
            }

            if (Input.GetKey(KeyCode.LeftShift) && !isCollidingWithObstacle && sprintTime > 0.2)
            {
                isSprinting = true;
                sprintTime -= Time.deltaTime;
                movementSpeed = sprintSpeed;
                playerCameraAnim.SetBool("isRunning", true);
                playerBodyAnim.SetBool("isRunning", true);
                sprintEffect.SetBool("Sprint", true);
                UImanager.UIPlayerSprinting();
                UImanager.PlayerSoundState = UImanager.PlayerSoundStateSprites[2];
            }
            else
            {
                movementSpeed = defaultMovementSpeed;
                sprintEffect.SetBool("Sprint", false);
                playerCameraAnim.SetBool("isRunning", false);
                playerBodyAnim.SetBool("isRunning", false);
                UImanager.UIPlayerDefaultState();


                if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    isSprinting = false;
                    if (sprintTime < 0.2)
                    {
                        FindObjectOfType<PlayerAnimationEvent>().RunPanting();
                    }
                }

                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    sprintTime += Time.deltaTime;
                    movementSpeed = defaultMovementSpeed;
                }
            }




            if (Input.GetKey(KeyCode.LeftControl))
            {
                PlayerCreep();
            }
            else
            {
                //movementSpeed = defaultMovementSpeed;
                isCreeping = false;
                //move MainCamera Y0.50 & Z0.175
                Transform CameraDefaultPosition;
                float time = 1f;
                CameraDefaultPosition = GameObject.Find("CameraDefaultPosition").transform;
                GameObject.FindGameObjectWithTag("MainCamera").transform.DOLocalMove(new Vector3(0, .4f, .3f), .5f);
                time += Time.deltaTime;

                if (Input.GetKeyUp(KeyCode.LeftControl))
                {
                    UImanager.UIPlayerDefaultState();
                    playerCreepingCollider.enabled = true;
                    playerBodyAnim.SetBool("isCrouching", false);
                    playerBodyAnim.SetBool("isCrouchingBackward", false);
                    playerCameraAnim.SetBool("isCreeping", false);

                    cc.center = new Vector3(0, -0.5f, 0.3f);
                    cc.radius = 0.75f;
                    cc.height = 2;
                }
            }

            if (Input.anyKey == false)
            {
                UImanager.PlayerSoundState = UImanager.PlayerSoundStateSprites[0];
            }
        }

        #endregion End Player Movement

        if (isTrapped)
        {
            Trapped();
        }

        #region PlayerPickingUp
        if (Input.GetKeyDown(KeyCode.E) && isInteracting == true) // i dont think this is activating reguarly. the isInteracting is probably responsible
        {
            playerCameraAnim.SetBool("isInteracting", true);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            playerCameraAnim.SetBool("isInteracting", false);
            isInteracting = false;
        }
        #endregion PlayerPickingUp

        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine("TorchInteract");
        }

        if (Input.GetMouseButtonDown(1))
        {
            CheckInventory();
        }

        #region Sprint Value Clamping
        if (sprintTime < 0)
        {
            sprintTime = 0;
        }

        if (sprintTime > maxSprintTime)
        {
            sprintTime = maxSprintTime;
        }

        #endregion Sprint Value Clamping
    }

    private void FixedUpdate()
    {
        cc.Move(movement * defaultMovementSpeed * Time.fixedDeltaTime);
    }

    void PlayerCreep()
    {
        isCreeping = true;
        movementSpeed = creepSpeed;
        //move MainCamera Y0.25 & Z0.5

        Transform CameraCreepingPosition;
        float time = 1f;
        CameraCreepingPosition = GameObject.Find("CameraCreepingPosition").transform;
        GameObject.FindGameObjectWithTag("MainCamera").transform.DOLocalMove(new Vector3(0, 0, 0.5f), .5f);//transform.position = Vector3.Slerp(transform.position, CameraCreepingPosition.position, time);
        time += Time.deltaTime;
        UImanager.UIPlayerCreeping();
        playerCreepingCollider.enabled = false;
        noiseRange.radius = noiseRangeIdle;
        playerBodyAnim.SetBool("isCrouching", true);
        cc.center = new Vector3(0, -1, 0.3f);
        cc.radius = 0.5f;
        cc.height = 1;

        if (Input.GetKey(KeyCode.W))
        {
            //playerCreepingCollider.enabled = true;
            playerCameraAnim.SetBool("isCreeping", true);
            playerBodyAnim.SetBool("isCrouchingBackward", false);
            noiseRange.radius = noiseRangeCreeping;
            UImanager.PlayerSoundState = UImanager.PlayerSoundStateSprites[0];
        }
        if (Input.GetKey(KeyCode.S))
        {
            //playerCreepingCollider.enabled = true;
            playerBodyAnim.SetBool("isCrouchingBackward", true);
            noiseRange.radius = noiseRangeCreeping;
            UImanager.PlayerSoundState = UImanager.PlayerSoundStateSprites[0];
        }
    }


    public IEnumerator TorchInteract()
    {
        playerCameraAnim.SetBool("isInteracting", true);
        //do torch animation

        yield return new WaitForSeconds(.5f);
        FindObjectOfType<AudioManager>().PlaySound("Torch");
        if (Torch.enabled == false)
        {
            Torch.enabled = true;
        }
        else
        {
            Torch.enabled = false;
        }
        playerCameraAnim.SetBool("isInteracting", false);
    }

    void Trapped()
    {
        playerCameraAnim.SetBool("isMoving", false);
        playerCameraAnim.SetBool("isRunning", false);
        //TEST - maybe play sound and new animation
    }

    void CheckInventory()
    {
        InventoryManager invMan = FindObjectOfType<InventoryManager>();
        if (!invMan.droppedInventory) // create world object for inventory 
                                       // once player picks up object. droppedInventory = false;
        {
            if (playerCameraAnim.GetBool("OpenInventory") == false && playerCameraAnim.GetBool("CheckItemInventory") == false)
            {
                checkInventory = true;
                movementSpeed = stoppedSpeed;
                playerCameraAnim.SetBool("OpenInventory", true);
                playerCameraAnim.SetBool("CheckItemInventory", false);
                invMan.InventoryCheck();
                invMan.inventoryOpen = true;
            }
            else
            {
                FindObjectOfType<InventoryManager>().inventorySelection = 0;
                checkInventory = false;
                playerCameraAnim.SetBool("OpenInventory", false);
                playerCameraAnim.SetBool("CheckItemInventory", false);

                UImanager.UIObjectDisable();

                invMan.InventorySelect();
                invMan.inventoryOpen = false;
                movementSpeed = defaultMovementSpeed;
            }
        }
        else
        {
            invMan.StartCoroutine("InventoryFull");
        }
    }
}
