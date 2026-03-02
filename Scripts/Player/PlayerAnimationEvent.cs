using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private AudioManager Aud;
    [SerializeField]
    private AudioClip[] AudSRandom;
    private AudioSource AudS;
    private PlayerSimpleMovement psm;

    private int AudSRandomRoll;

    [SerializeField]
    private float runVolume;
    [SerializeField]
    private float walkVolume;
    [SerializeField]
    private float creepVolume;

    [Range(0.5f, 1.5f)]
    private float pitch = 1.0f;

    [Range(0f, 0.5f)]
    private float randomPitch = 0.1f;

    private void Start()
    {
        Aud = FindObjectOfType<AudioManager>();
        AudS = GetComponent<AudioSource>();
        psm = FindObjectOfType<PlayerSimpleMovement>();
    }

    void WalkFootstep()
    {


        AudSRandomRoll = Random.Range(0, AudSRandom.Length);
        AudS.clip = AudSRandom[AudSRandomRoll];
        AudS.volume = walkVolume;
        AudS.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, +randomPitch / 2f));
        AudS.Play();
        StartCoroutine("WalkSound");
    }

    void RunFootstep()
    {
        AudSRandomRoll = Random.Range(0, AudSRandom.Length);
        AudS.clip = AudSRandom[AudSRandomRoll];
        AudS.volume = runVolume;
        AudS.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, +randomPitch / 2f));
        AudS.Play();
        StartCoroutine("RunSound");
    }

    public void RunPanting()
    {
        if (!Aud.IsSoundPlaying("RunPanting"))
        {
            Aud.PlaySound("RunPanting");
        }

    }

    void CreepFootstep()
    {
        AudSRandomRoll = Random.Range(0, AudSRandom.Length);
        AudS.clip = AudSRandom[AudSRandomRoll];
        AudS.volume = creepVolume;
        AudS.Play();
        StartCoroutine("CreepSound");
    }

    void HandImpact()
    {
        Aud.PlaySound("HandImpact");
    }

    void CheckInventory()
    {
        Aud.PlaySound("CheckInventory");
    }

    void CheckItemInventory()
    {
        Aud.PlaySound("CheckItemInventory");
    }



    public IEnumerator CreepSound()
    {
        psm.noiseRange.radius = psm.noiseRangeCreeping;
        yield return new WaitForSeconds(0.25f);
        psm.noiseRange.radius = psm.noiseRangeIdle;
    }

    public IEnumerator WalkSound()
    {
        psm.noiseRange.radius = psm.noiseRangeWalking;
        yield return new WaitForSeconds(0.25f);
        psm.noiseRange.radius = psm.noiseRangeIdle;
    }

    public IEnumerator RunSound()
    {
        psm.noiseRange.radius = psm.noiseRangeRunning;
        yield return new WaitForSeconds(0.25f);
        psm.noiseRange.radius = psm.noiseRangeIdle;
    }
}
