using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject Menu;

    [SerializeField]
    private GameObject Option;

    [SerializeField]
    private GameObject Controls;

    [SerializeField]
    private GameObject LevelSelect;

    public string levelToSelect;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            string folderPath = "Assets/Screenshots/"; // the path of your project folder

            if (!System.IO.Directory.Exists(folderPath)) // if this path does not exist yet
                System.IO.Directory.CreateDirectory(folderPath);  // it will get created

            var screenshotName =
                                    "Screenshot_" +
                                    System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + // puts the current time right into the screenshot name
                                    ".png"; // put youre favorite data format here
            ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName), 2); // takes the sceenshot, the "2" is for the scaled resolution, you can put this to 600 but it will take really long to scale the image up
            Debug.Log(folderPath + screenshotName); // You get instant feedback in the console
        }

        if(Input.anyKeyDown && SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Credits"))
        {
            LoadMenu();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            LoadLevel();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public void LoadMenu()
    {
        Debug.Log("Load Level " + SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene("Menu");
    }

    public void LoadLevel()
    {
        Debug.Log("Load Level " + SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene("Intro");
    }

    public void SkipIntro()
    {
        SceneManager.LoadScene("1Mansion");
    }
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void Options()
    {
        //disable menu UI gameobject
        Menu.SetActive(false);
        //enable UI gameobject for options
        Option.SetActive(true);
    }

    public void OptionsBack()
    {
        Menu.SetActive(true);
        //disable menu UI gameobject
        Option.SetActive(false);
        //enable UI gameobject for options

    }
    public void Control()
    {
        //disable menu UI gameobject
        Menu.SetActive(false);
        //enable UI gameobject for options
        Controls.SetActive(true);
    }

    public void ControlsBack()
    {
        Menu.SetActive(true);
        //disable menu UI gameobject
        Controls.SetActive(false);
        //enable UI gameobject for options

    }

    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void Levels()
    {
        Menu.SetActive(false);
        //disable menu UI gameobject
        LevelSelect.SetActive(true);
        //enable UI gameobject for options
    }

    public void SelectLevel()
    {
        TMP_InputField inputField = LevelSelect.GetComponentInChildren<TMPro.TMP_InputField>();

        string userInput = inputField.text;

        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);

            if (sceneName.Length > 1)
            {
                string trimmedSceneName = sceneName.Substring(1); // Remove first character

                if (trimmedSceneName.Equals(userInput, System.StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log($"Loading scene: {sceneName}");
                    SceneManager.LoadScene(sceneName);
                    return;
                }
            }
        }


            //levelToSelect = LevelSelect.GetComponentInChildren<TMPro.TMP_InputField>().text;
        //SceneManager.LoadScene(levelToSelect); 
    }

    public void LevelsBack()
    {
        Menu.SetActive(true);
        //enable menu UI gameobject
        LevelSelect.SetActive(false);
        //disable UI gameobject for options
    }

    IEnumerator LoadLevel1()
    {
        Debug.Log("Load Level " + SceneManager.GetActiveScene().buildIndex + 1);
        yield return new WaitForSeconds(1);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene("Intro");
    }

    IEnumerator SkipIntro1()
    {
        Debug.Log("Load Level " + SceneManager.GetActiveScene().buildIndex + 1);
        yield return new WaitForSeconds(1);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene("Intro");
    }

    IEnumerator QuitGame1()
    {
        yield return new WaitForSeconds(1f);
        Application.Quit();
    }

    public void LoadSteam()
    {
        Application.OpenURL("https://store.steampowered.com/");
    }
    public void LoadSocial()
    {
        Application.OpenURL("https://x.com/shrek2ondvd");
    }
    public void LoadYoutube()
    {
        Application.OpenURL("https://www.youtube.com/@layers4667");
    }
    public void LoadSurvey()
    {
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSd4usDdZ3mkmeegQbpy2iuY-zhVsCfoiygwoeh7HeWkpcNsoQ/viewform?usp=header7");
    }
    
}
