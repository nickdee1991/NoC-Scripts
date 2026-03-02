using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleInteractable : MonoBehaviour
{
    public bool playerInRange;

    public string ObjectName;

    private UIManager uiManager;
    private PlayerSimpleMovement playerMovement;
    [SerializeField]
    private Interactable interactable;
    [SerializeField]
    private GameObject puzzleObject;
 

    private bool puzzleComplete;
    private bool isInPuzzle;
    private bool puzzleCooldown;

    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        playerMovement = FindObjectOfType<PlayerSimpleMovement>();
        if (puzzleObject.activeSelf == true)
        {
            puzzleObject.SetActive(false);
        }
        puzzleComplete = false;
        isInPuzzle = false;
        puzzleCooldown = false;
    }

    public void StartPuzzle()
    {
        if (puzzleCooldown || isInPuzzle)
            return;
        isInPuzzle= true;   
        puzzleCooldown = true;
        playerMovement.movementSpeed = 0;
        playerMovement.isTrapped = true;
        puzzleObject.SetActive(true);

        interactable.enabled = false;
        uiManager.UIPuzzleObject();

        StartCoroutine("PuzzleCooldown");
    }

    public void StopPuzzle()
    {
        if (puzzleCooldown)
            return;
        puzzleCooldown = true;

        playerMovement.movementSpeed = playerMovement.defaultMovementSpeed;
        playerMovement.isTrapped = false;
        isInPuzzle = false;
        interactable.enabled = true;
        puzzleObject.SetActive(false);
        uiManager.StopUIPuzzleObject();

        StartCoroutine("PuzzleCooldown");
    }

    public void CompletePuzzle()
    {
        if (puzzleCooldown)
            return;
        puzzleCooldown = true;
        puzzleComplete = true;
        playerMovement.movementSpeed = playerMovement.defaultMovementSpeed;
        playerMovement.isTrapped = false;
        isInPuzzle = false;
        interactable.enabled = true;
        puzzleObject.SetActive(false);
        uiManager.StopUIPuzzleObject();

        Debug.Log("Puzzle Complete");
        StartCoroutine("PuzzleCooldown");
    }

    public IEnumerator PuzzleCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        puzzleCooldown = false;
    }
}
