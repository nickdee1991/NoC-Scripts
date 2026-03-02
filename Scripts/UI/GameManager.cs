using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public bool DEBUGMODE;

    public bool screenshotMode;

    public bool StartPlayerAtSpawn;

    public GameObject[] objectsToEnable;

    public GameObject[] disablePlayerUI;

    private GameObject player;

    public GameObject enemyThatCapturedPlayer;

    public GameObject InventoryObject;

    public GameObject MenuCanvas;

    [SerializeField]
    private GameObject enemyCaptured;
    [SerializeField]
    private CapturedScene CapturedScene;

    public Transform[] spawnPoints;
    public Transform startSpawn;
    public Transform playerCapturedSpawn;
    public Transform enemyCapturedSpawn;

    public bool trapPlaced;

    [SerializeField]
    public Enemy[] allEnemiesInGame;

    [SerializeField]
    private AudioSource CapturedSound;

    [SerializeField]
    private AudioSource AlertedSound;
    [SerializeField]
    private float AlertedSoundDefault;

    public AudioClip[] sound;
    private AudioClip soundPlaying;

    private AudioManager Aud;
    [SerializeField]
    private UIManager UImanager;
    [SerializeField]
    private RandomSoundManager randSoundManager;

    public bool isGameOver;
    [SerializeField]
    private bool dontDestroyOnLoad;

    public string LevelName;
    public string AlternateLevelName;
    public string LevelCompleteSound;

    private Animator anim;

    public int timesCaptured = 0;
    public float levelCompleteWaitTime = 3;

    public float exitTime;

    private void Awake()
    {
        Aud = FindObjectOfType<AudioManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        randSoundManager = GetComponent<RandomSoundManager>();
        AlertedSoundDefault = AlertedSound.volume;
        //NavMesh.RemoveAllNavMeshData(); //use this if the navmesh wont clear on re-bake

        if (StartPlayerAtSpawn)
        {
            Debug.Log("start player at spawn");
            player.transform.SetPositionAndRotation(startSpawn.transform.position, startSpawn.transform.rotation);
            player.GetComponent<CharacterController>().enabled = true;
        }
    }

    void Start()
    {
        List<Enemy> enemiesThatSeePlayer = new List<Enemy>();
        playerCapturedSpawn = GameObject.Find("PlayerCapturedSpawn").transform;
        enemyCapturedSpawn = GameObject.Find("EnemyCapturedSpawn").transform;
        CapturedScene = FindObjectOfType<CapturedScene>();
        UImanager = FindObjectOfType<UIManager>();
        allEnemiesInGame = FindObjectsOfType<Enemy>();
        Cursor.visible = false;
        anim = GameObject.Find("Main Camera").GetComponent<Animator>();
        player.GetComponent<CharacterController>().enabled = true;

        StartCoroutine("CheckForEnemiesActive");
        StartCoroutine("LateStart");
        GameObject.Find("TransitionLevelName").GetComponent<TextMeshProUGUI>().text = SceneManager.GetActiveScene().name.Substring(1);
        if (screenshotMode)
        {
            foreach (GameObject UI in disablePlayerUI) // any objects that arent needed in the editor (eg background terrain, anything outside player bounds)
                                                                   // go in this container to be re-enabled on play
            {
                UI.SetActive(false);
            }
        }

        trapPlaced = false;

        foreach (GameObject objectDisabled in objectsToEnable) // any objects that arent needed in the editor (eg background terrain, anything outside player bounds)
            // go in this container to be re-enabled on play
        {
            objectDisabled.SetActive(true);
        }

        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

        isGameOver = false;
    }

    public IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.1f);
        Aud.PlaySound("Intro");//for some reason this needs to be here, i think it has something to do with this sound playing before AudioManager has added all 30+ sounds to the game on startup, creating a null exception 
    }

    private void FixedUpdate() // runs every 0.02secs
    {
        CheckIfPlayerSpotted();
    }

    private void Update()
    {
        UImanager.ExitTime.gameObject.SetActive(false);

        if (MenuCanvas.activeInHierarchy)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }

        if (timesCaptured >= 3 && !isGameOver)
        {
            GameOver();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            //Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //UImanager.ExitTime.gameObject.SetActive(true);
            //SceneManager.LoadScene("Menu");
            //exitTime -= Time.deltaTime;

            if (!MenuCanvas.activeInHierarchy)
            {
                MenuCanvas.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                MenuCanvas.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
// some clamping bullshit
    }

    IEnumerator EscapeDelay() //adds enemies not spawned on runtime to the "All enemies in game" array. So they will trigger the 'detected' ui
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Menu");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void RestartLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    void CheckIfPlayerSpotted()
    {
        for (int i = 0; i < allEnemiesInGame.Length; i++)
        {
            if (!allEnemiesInGame[i].detectedEnemy)
            {
                //Debug.Log("no one can see player");
                UImanager.UIPlayerNotSpotted();
            }
        }

        foreach (Enemy enemy in allEnemiesInGame)  // loop through every enemy script in the scene
        {
            if (enemy.CanSeePlayer() && enemy.detectedEnemy) // if this enemy has seen player
            {
                if (!AlertedSound.isPlaying)//!Aud.IsSoundPlaying("Attack")
                { 
                    RandomAlertedSound();
                }


                if (enemy.detectedEnemy)
                {
                    Debug.Log(enemy.gameObject.name + " is pursuing player");
                    UImanager.UIPlayerPursued(); // change player UI to reflect that enemy is pursuing you
                }

            }
            if (enemy.CanSeePlayer() && !enemy.detectedEnemy) // if the enemy can see the player - but is not pursuing
            {
                //Debug.Log(enemy.gameObject.name + " can see player");
                UImanager.UIPlayerSpotted(); // change player UI to reflect that enemy is can see you

                if (!Aud.IsSoundPlaying("Violin"))
                {
                    Aud.PlaySound("Violin");
                }

            }

            if(!enemy.CanSeePlayer() && !enemy.detectedEnemy && !enemy.searchingForPlayer && !Aud.IsSoundPlaying("Attack"))
            {
                //Debug.Log("Fading out sound ");
                Aud.FadeOutSound("Attack");
            }
        }
    }

    public void RandomAlertedSound()
    {
        int index = Random.Range(0, sound.Length);
        soundPlaying = sound[index];
        AlertedSound.clip = soundPlaying;
        AlertedSound.volume = AlertedSoundDefault;
        AlertedSound.Play();
    }

    public void Captured()
    {
        if (timesCaptured < 3 && !isGameOver)
        {
            if(FindObjectOfType<InventoryManager>().droppedInventory == false)
            {
                FindObjectOfType<InventoryManager>().droppedInventory = true;
                Instantiate(InventoryObject, player.transform.position + transform.forward * 1.5f - transform.up, Quaternion.identity);
            }
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.SetPositionAndRotation(playerCapturedSpawn.transform.position, playerCapturedSpawn.transform.rotation);
            Debug.Log("Captured " + timesCaptured);

            StartCoroutine("CapturedTimer");
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over " + timesCaptured);
        isGameOver = true;
        StartCoroutine("GameOverTimer");
    }

    IEnumerator CheckForEnemiesActive() //adds enemies not spawned on runtime to the "All enemies in game" array. So they will trigger the 'detected' ui
    {
        allEnemiesInGame = FindObjectsOfType<Enemy>();
        //Debug.Log("Checking for active enemies"); 
        yield return new WaitForSeconds(1f);
        StartCoroutine("CheckForEnemiesActive");
    }
    public static class FadeAudioSource // good template for fading anything
    {
        public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
        {
            float currentTime = 0;
            float start = audioSource.volume;
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
                yield return null;
            }
            yield break;
        }
    }
    IEnumerator CapturedTimer()
    {
        player.GetComponent<PlayerSimpleMovement>().movementSpeed = 0;
        CapturedScene.enemyThatCaptured = enemyThatCapturedPlayer.GetComponentInChildren<Enemy>().enemyNumber;
        enemyThatCapturedPlayer.GetComponentInChildren<EnemyVoiceManager>().PlayRandomCapturedVoice();
        CapturedSound.clip = enemyThatCapturedPlayer.GetComponentInChildren<EnemyVoiceManager>().soundPlaying;
        CapturedSound.Play();

        CapturedScene.CapturedSceneTrigger();

        Aud.PlaySound("Violin");
        yield return new WaitForSeconds(3.5f);
        Aud.StopSound("Violin");
        timesCaptured++;

        player.GetComponent<CharacterController>().enabled = true;
        Debug.Log("Captured " + timesCaptured);
        //enemyThatCapturedPlayer.GetComponentInChildren<Enemy>().IdleEnemy(); // does this need to exist?

        yield return new WaitForSeconds(5f);
        StartCoroutine(FadeAudioSource.StartFade(AlertedSound, 1f, 0f));
        enemyThatCapturedPlayer = null;
        CapturedSound.clip = null;
        CapturedSound.Stop();
        player.GetComponent<PlayerCaught>().Captured = false;
    }


    IEnumerator GameOverTimer()
    {
        anim.SetBool("CapturedTransition", true);
        Aud.PlaySound("GameOverVoice");
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Menu");
    }

    public void LevelComplete()
    {
        StartCoroutine("LevelCompleteTimer");
    }
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    IEnumerator LevelCompleteTimer()
    {
        GetComponentInChildren<Animator>().SetTrigger("Start");//fade to black
        FindObjectOfType<AudioManager>().PlaySound(LevelCompleteSound);       //play level end sound
        GameObject.Find("UIText").SetActive(false);
        Debug.Log("Level Complete. Next level is " + SceneManager.GetActiveScene().buildIndex + 1);
        yield return new WaitForSeconds(levelCompleteWaitTime);
        SceneManager.LoadScene(LevelName);
    }

    public void AlternateLevel()
    {
        StartCoroutine("AlternateLevelTimer");        //move level according to objective string "AlternativeLevel"
    }

    IEnumerator AlternateLevelTimer()
    {
        float levelCompleteWaitTime = 3;
        yield return new WaitForSeconds(levelCompleteWaitTime);
        SceneManager.LoadScene(AlternateLevelName);
    }

}
