using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TypeWriterEffectEpilogue : MonoBehaviour {

    public float delay = 0.1f;
    private string fullText;
    private string currentText = "";
    private AudioSource Aud;

    private void Start()
    {
        Aud = GetComponent<AudioSource>();
        StartCoroutine(ShowText());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene("Menu");
        }
    }

    IEnumerator ShowText()
    {
        for(int i = 0; i < fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            GetComponent<Text>().text = currentText;
            yield return new WaitForSeconds(delay);            
        }
        yield return null;
        Aud.Stop();
    }
}
