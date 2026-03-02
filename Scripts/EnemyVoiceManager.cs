using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVoiceManager : MonoBehaviour
{
    [SerializeField]
    public AudioClip soundPlaying;

    public AudioSource aud;
    [SerializeField]
    private AudioClip[] patrolSounds;

    [SerializeField]
    private AudioClip[] investigateSounds;

    [SerializeField]
    private AudioClip[] pursuingSounds;

    [SerializeField]
    private AudioClip[] conversingSounds;

    [SerializeField]
    private AudioClip[] capturedSounds;

    private Enemy enemy;
    private PatrolRandom patrolRandom;

    public float patrolWaitTime;
    public float pursuingWaitTime;
    public float conversingWaitTime;

    private int conversingNumber;


    public float conversationLength; // get a clips length

    [SerializeField]
    private bool playOnAwake;

    public bool playOnTrigger;

    public bool patrolling;
    public bool investigating;
    public bool pursuing;
    public bool conversing;
    public bool conversingDialogue;

    private void Start()
    {
        if (playOnAwake)
        {
            StartCoroutine("PatrolVoice");
        }

        aud = GetComponent<AudioSource>();
        enemy = GetComponentInParent<Enemy>();
        patrolRandom = GetComponentInParent<PatrolRandom>();
    }

    private void LateUpdate()
    {
        if(enemy.CurrentEnemyState == Enemy.EnemyState.PATROLLING && !patrolling)
        {
            StartCoroutine("PatrolVoice");
        }

        if (enemy.CurrentEnemyState == Enemy.EnemyState.PURSUING && !pursuing)
        {
            StartCoroutine("PursuingVoice");
        }

        if (enemy.CurrentEnemyState == Enemy.EnemyState.CONVERSING && !conversing)
        {
            if(playOnAwake || playOnTrigger)
            {
                StartCoroutine("ConversingVoice");
            }

        }

        if (enemy.CurrentEnemyState == Enemy.EnemyState.INVESTIGATING && !investigating)
        {
            //InvestigationVoice();
            StartCoroutine("InvestigationVoice");
        }
    }

    public void PlayRandomCapturedVoice()
    {
        StopCoroutine("PursuingVoice");
        StopCoroutine("PatrolVoice");
        StopCoroutine("ConversingVoice");

        aud.Stop();

        if (!aud.isPlaying)
        {
            int index = Random.Range(0, capturedSounds.Length);
            soundPlaying = capturedSounds[index];
            aud.clip = soundPlaying;
            Debug.Log("Sound playing " +soundPlaying.name);
            aud.Play();
        }
    }


    public void PlayRandomPatrolVoice()
    {
        if (!aud.isPlaying)
        {
            int index = Random.Range(0, patrolSounds.Length);
            soundPlaying = patrolSounds[index];
            aud.clip = soundPlaying;
            aud.Play();
        }
    }

    public void PlayRandomPursuingVoice()
    {
        if (!aud.isPlaying)
        {
            int index = Random.Range(0, pursuingSounds.Length);
            soundPlaying = pursuingSounds[index];
            aud.clip = soundPlaying;
            aud.Play();
        }
    }

    public void PlayRandomInvestigatingVoice()
    {
        if (!aud.isPlaying)
        {
            int index = Random.Range(0, investigateSounds.Length);
            soundPlaying = investigateSounds[index];
            aud.clip = soundPlaying;
            aud.Play();
        }

    }

    public IEnumerator PatrolVoice()
    {
        investigating = false;
        pursuing = false;
        patrolling = true;
        StopCoroutine("PursuingVoice");

        while (true)
        {
            yield return new WaitForSeconds(patrolWaitTime);
            int index = Random.Range(0, patrolSounds.Length);
            soundPlaying = patrolSounds[index];
            aud.clip = soundPlaying;
            aud.Play();
        }

    }

    public IEnumerator ConversingVoice()
    {
        investigating = false;
        pursuing = false;
        patrolling = false;
        conversing = true;
;
        StopCoroutine("PursuingVoice");
        StopCoroutine("PatrolVoice");

        if (!aud.isPlaying)
        {
            //have both conversers check the audio is playing and then resume patrol when done
            yield return new WaitForSeconds(conversingWaitTime);
            //Debug.Log("conversing");
            conversingDialogue = true;
            int index = Random.Range(0, conversingSounds.Length);
            soundPlaying = conversingSounds[index];
            aud.clip = soundPlaying;
            float length = aud.clip.length;
            conversationLength = length;
            Debug.Log("Sound playing " + soundPlaying.name + " for " + length);
            aud.Play();
        }

    }

    public void InvestigationVoice()
    {
        investigating = true;
        patrolling = false;
        pursuing = false;

        StopCoroutine("PursuingVoice");
        StopCoroutine("PatrolVoice");

        if (!aud.isPlaying)
        {
            int index = Random.Range(0, investigateSounds.Length);
            soundPlaying = investigateSounds[index];
            aud.clip = soundPlaying;
            aud.Play();
        }
    }

    public IEnumerator PursuingVoice()
    {
        investigating = false;
        patrolling = false;
        pursuing = true;

        StopCoroutine("PatrolVoice");

        while (true)
        {
            yield return new WaitForSeconds(pursuingWaitTime);
            int index = Random.Range(0, pursuingSounds.Length);
            soundPlaying = pursuingSounds[index];
            aud.clip = soundPlaying;
            aud.Play();
        }
    }
}
