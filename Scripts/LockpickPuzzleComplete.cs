using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockpickPuzzleComplete : MonoBehaviour
{
    [SerializeField]
    private GameObject pick;

    [SerializeField]
    private GameObject[] pins;

    [SerializeField]
    private PuzzleInteractable puzzleComplete;
    [SerializeField]
    private GameObject puzzleObject;

    [SerializeField]
    private string scriptToInteractWith; // type what script the player will interact with.

    [SerializeField]
    private string methodInsideInteractedScript; // the name for the method you want to activate in the object

    [SerializeField]
    private GameObject objectThatPuzzleUnlocks;

    private AudioSource aud;

    private ParticleSystem puzzleCompleteParticles;

    private bool puzzleCompleteCheck;

    private void Start()
    {
        aud = GetComponent<AudioSource>();
        puzzleCompleteParticles = GetComponentInChildren<ParticleSystem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Pick") && !puzzleCompleteCheck)
        {
            StartCoroutine("CompletePuzzle");
        }
    }

    public IEnumerator CompletePuzzle()
    {
        //play sound
        aud.Play();
        //play particle effect
        puzzleCompleteParticles.Play();
        //stop puzzle moving parts
        puzzleCompleteCheck = true;

        foreach (GameObject pin in pins)
        {
            pin.GetComponent<Animator>().enabled = false;
        }

        yield return new WaitForSeconds(2f);
        puzzleObject.SetActive(false);
        objectThatPuzzleUnlocks.GetComponent(scriptToInteractWith).BroadcastMessage(methodInsideInteractedScript);
        puzzleComplete.CompletePuzzle();
    }
}
