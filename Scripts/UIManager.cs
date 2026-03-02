using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Rendering.PostProcessing;
using TMPro;

public class UIManager : MonoBehaviour
{
    private PlayerSimpleMovement playerMovement;
    private CameraLook cameraLook;

    private GameManager gm;

    public Image PlayerSound;
    public Sprite PlayerSoundState;
    public Sprite[] PlayerSoundStateSprites;

    public Image PlayerAlert;
    public Sprite PlayerAlertState;
    public Sprite[] PlayerAlertStateSprites;

    public Image PlayerCrosshair;
    public Sprite PlayerCrosshairState;
    public Sprite[] PlayerCrosshairStateSprites;

    public Image PlayerCaptured;

    public TextMeshProUGUI SprintText; // this is a test for UI
    public Slider SprintBar;
    public Slider ExitTime;
    public Slider ObjectTime;
    [SerializeField]
    private TextMeshProUGUI loreText;
    public Item loreObject;
    private RawImage loreBG;
    private RawImage loreBGPage;

    [SerializeField]
    private TextMeshProUGUI puzzleText;
    private RawImage puzzleBG;

    public TextMeshProUGUI objectText;
    public GameObject objectObjectText; // the gameobject holding the text and the bg
    public string objecttextText;
    public TextMeshProUGUI tmpText; // Reference to the TextMeshPro component
    public RectTransform rectTransform; // Reference to the RectTransform


    private Tweener AlertTween;
    private Tweener AlertTween2;

    static float t = 0.0f;

    private void Start()
    {

        AlertTween = PlayerAlert.transform.DOPunchScale(new Vector3(0.25f, 0.25f, 0.25f), 1, 1, 0.5f).SetAutoKill(false);
        AlertTween2 = PlayerAlert.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 1, 1, 0.5f).SetAutoKill(false);

        playerMovement = FindObjectOfType<PlayerSimpleMovement>();
        cameraLook = FindObjectOfType<CameraLook>();

        PlayerSoundState = PlayerSoundStateSprites[1];
        PlayerSound.sprite = PlayerSoundState;

        PlayerAlertState = PlayerAlertStateSprites[0];
        PlayerAlert.sprite = PlayerAlertState;

        PlayerCrosshairState = PlayerCrosshairStateSprites[0];
        PlayerCrosshair.sprite = PlayerCrosshairState;

        gm = FindObjectOfType<GameManager>();
        loreText = GameObject.Find("LoreText").GetComponent<TextMeshProUGUI>();
        loreBG = GameObject.Find("LoreBG").GetComponent<RawImage>();
        loreBGPage = GameObject.Find("LoreBGPage").GetComponent<RawImage>();
        objectText = GameObject.Find("ObjectTextHolder").GetComponentInChildren<TextMeshProUGUI>();
        objectObjectText = GameObject.Find("ObjectTextHolder");
        objecttextText = objectText.text;
        UIObjectDisable();

        SprintBar.maxValue = playerMovement.maxSprintTime;
        SprintBar.minValue = 0;

        ExitTime.maxValue = 2f;
        ExitTime.minValue = 0;

        ObjectTime.maxValue = 0.5f;
        ObjectTime.minValue = 0;
    }

    private void Update()
    {
        PlayerSound.sprite = PlayerSoundState;
        PlayerAlert.sprite = PlayerAlertState;

        PlayerCrosshair.sprite = PlayerCrosshairState;
        if (PlayerCrosshairState == PlayerCrosshairStateSprites[0])
        {
            PlayerCrosshair.transform.localScale= new Vector3(0.1f, 0.1f, 0.1f);
        }else // forgive me code god
        {
            PlayerCrosshair.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        SprintBar.value = playerMovement.sprintTime;
        ExitTime.value = gm.exitTime;
        if (cameraLook.itemInRange.GetComponentInChildren<HidingSpot>())
        {
            ObjectTime.value = cameraLook.itemInRange.GetComponentInChildren<HidingSpot>().hidingSpotDelay;
        }

        if (!AlertTween.IsPlaying())
        {
            AlertTween.Rewind();
            AlertTween.Play();
        }

        t += 0.1f * Time.deltaTime;
    }

    public void UIObjectEnable()
    {
        //objectText.enabled = true;
        objectObjectText.SetActive(true);

        // Update the size of the RectTransform based on the text's preferred size
        Vector2 newSize = new Vector2(tmpText.preferredWidth, tmpText.preferredHeight);
        rectTransform.sizeDelta = newSize;
    }

    public void UIObjectDisable()
    {
        objectText.text = "";
        objectObjectText.SetActive(false);
        //objectText.enabled = false;
    }

    public void UICrossHairSet()
    {
        PlayerCrosshairState = PlayerCrosshairStateSprites[1]; // this might need to be rebuilt to account for different crosshair types
    }
    public void UICrossHairUnset()
    {
        PlayerCrosshairState = PlayerCrosshairStateSprites[0];
    }

    public void UIPlayerPursued()
    {
        if (!AlertTween2.IsPlaying())
        {
            AlertTween2.Rewind();
            AlertTween2.Play();
        }

        //alerted effect for player
        FindObjectOfType<PostProcessVolume>().profile.GetSetting<Vignette>().intensity.Override(0.35f);
        FindObjectOfType<PostProcessVolume>().profile.GetSetting<Vignette>().color.value = Color.red; //F73859
        //FindObjectOfType<PostProcessVolume>().profile.GetSetting<DepthOfField>().focusDistance.Override(0.5f);
        //alerted effect for player

        PlayerAlertState = PlayerAlertStateSprites[2];
    }

    #region // vignette for movement
    public void UIPlayerDefaultState()
    {
        FindObjectOfType<PostProcessVolume>().profile.GetSetting<Vignette>().intensity.Override(Mathf.Lerp(0.25f, 0.25f, t));
    }

    public void UIPlayerSprinting()
    {

        FindObjectOfType<PostProcessVolume>().profile.GetSetting<Vignette>().intensity.Override(Mathf.Lerp(0.25f, 0.35f, t));
    }
    public void UIPlayerCreeping()
    {
        FindObjectOfType<PostProcessVolume>().profile.GetSetting<Vignette>().intensity.Override(Mathf.Lerp(0.25f, 0.3f, t));

    }
    #endregion

    public void UIPlayerSpotted()
    {
        if (!AlertTween.IsPlaying())
        {
            AlertTween.Rewind();
            AlertTween.Play();
        }

        PlayerAlertState = PlayerAlertStateSprites[1];
    }

    public void UIPlayerNotSpotted()
    {
        if (!AlertTween.IsPlaying())
        {
            AlertTween.Pause();
        }
        if (!AlertTween2.IsPlaying())
        {
            AlertTween.Pause();
        }
        FindObjectOfType<PostProcessVolume>().profile.GetSetting<Vignette>().color.value = Color.black; //39435A
        PlayerAlertState = PlayerAlertStateSprites[0];
    }

    public void UILoreObject()
    {
        loreText.enabled = true;
        loreText.text = loreObject.loreTextText;
        loreBG.enabled = true;
        loreBGPage.enabled = true;
        FindObjectOfType<PostProcessVolume>().profile.GetSetting<Vignette>().intensity.Override(0.35f);
        FindObjectOfType<PostProcessVolume>().profile.GetSetting<DepthOfField>().focusDistance.Override(0.5f);
    }

    public void StopUILoreObject()
    {
        loreText.enabled = false;
        loreText.text = "";
        loreBG.enabled = false;
        loreBGPage.enabled = false;
        FindObjectOfType<PostProcessVolume>().profile.GetSetting<Vignette>().intensity.Override(0.25f);
        FindObjectOfType<PostProcessVolume>().profile.GetSetting<DepthOfField>().focusDistance.Override(3f);
    }

    public void UIPuzzleObject()
    {

        FindObjectOfType<PostProcessVolume>().profile.GetSetting<Vignette>().intensity.Override(0.35f);
        FindObjectOfType<PostProcessVolume>().profile.GetSetting<DepthOfField>().focusDistance.Override(0.1f);

        cameraLook.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void StopUIPuzzleObject()
    {

        FindObjectOfType<PostProcessVolume>().profile.GetSetting<Vignette>().intensity.Override(0.25f);
        FindObjectOfType<PostProcessVolume>().profile.GetSetting<DepthOfField>().focusDistance.Override(2f);

        cameraLook.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
