using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSound : MonoBehaviour
{
    [SerializeField]
    private string audioToPlay1;
    [SerializeField]
    private string audioToPlay2;
    [SerializeField]
    private string audioToPlay3;

    private AudioManager aud;

    public void Start()
    {
        aud = FindObjectOfType<AudioManager>();
    }
    void Sound1()
    {
        aud.PlaySound(audioToPlay1);
    }
    void Sound2()
    {
        aud.PlaySound(audioToPlay2);
    }
    void Sound3()
    {
        aud.PlaySound(audioToPlay3);
    }
}
