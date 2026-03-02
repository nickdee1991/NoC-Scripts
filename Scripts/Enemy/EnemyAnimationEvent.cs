using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvent : MonoBehaviour
{
    private AudioSource Aud;

    [SerializeField]
    private AudioClip[] footstepSounds;
    [SerializeField]
    private float footStepVolume = 0.1f;

    [SerializeField]
    private AudioClip[] trapSound;
    [SerializeField]
    private float trapVolume = 0.25f;

    [SerializeField]
    private Transform AnimRigTarget; // target for the animation rig - whatever this points to the head will look at
    private Transform AnimRigTargetPosition;
    [SerializeField]
    private GameObject HeadRig; // target for the animation rig - whatever this points to the head will look at
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private Enemy enemy;

    private void Start()
    {
        Aud = GetComponent<AudioSource>();
        enemy = GetComponent<Enemy>();
        if (AnimRigTarget != null)
        {
            AnimRigTargetPosition = AnimRigTarget.transform;
            HeadRig.transform.LookAt(AnimRigTargetPosition);
        }
    }

    private void FixedUpdate()
    {
        if (AnimRigTarget != null)
        {
            //AnimRigTarget.transform.position = AnimRigTargetPosition.transform.position;
            //HeadRig.transform.LookAt(AnimRigTargetPosition);

            if (enemy.detectedEnemy == true)
            {
                AnimRigTarget.transform.position = player.transform.position;// -- for when enemy spots player
            }

            if (enemy.investigatingEvidence)
            {
                AnimRigTarget.transform.position = enemy.InvestigateLocationPosition.transform.position;
            }
        }

    }

    void Footstep()
    {
        int index = UnityEngine.Random.Range(0, footstepSounds.Length);

        Aud.clip = footstepSounds[index];
        Aud.volume = footStepVolume;

        Aud.Play();
    }

    void Trap() 
    {
        int index = UnityEngine.Random.Range(0, trapSound.Length);

        Aud.clip = trapSound[index];
        Aud.volume = trapVolume;

        Aud.Play();
    }
}
