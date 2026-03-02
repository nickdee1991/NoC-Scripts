using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TypeWriterEffect : MonoBehaviour {

    public float startTextDelay;
    public float textDelay = 0.1f;
    public float endTextDelay;

    [TextArea(3, 10)]
    public string fullText;

    private string currentText = "";
    public TMPro.TextMeshProUGUI textToUse;
    public TypeWriterEffect nextTextToUse;

    //[SerializeField]
    //private RawImage textBox;

    private void Start()
    {
        //if(textBox != null)
        //{
            //textBox = GameObject.Find("CutsceneTextBox").GetComponent<RawImage>();
            //textBox.enabled = false;
        //}
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        //textBox.enabled = true;
        yield return new WaitForSeconds(startTextDelay);

        for (int i = 0; i < fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            textToUse.text = currentText;
            yield return new WaitForSeconds(textDelay);            
        }
        yield return new WaitForSeconds(endTextDelay);
        //textBox.enabled = false;
        if (nextTextToUse != null)
        {
            enabled = true;
            nextTextToUse.enabled = true;
        }
    }
}
