using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    //ui components to display info to the player
    public TextMeshProUGUI musicTitle;
    public TextMeshProUGUI levelTimer;
    public TextMeshProUGUI levelTimerCopy;
    public Animator uiAnimator;
    public Image cursor;
    public Image doubleJumpIndicator;
    public Slider dashIndicator;

    [Header("Pause Menu")]
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public Slider mouseSensitivity;
    public Slider sfxVolume;
    public Slider musicVolume;
    public Toggle usePostProcessing;

    [HideInInspector] public bool canLoadScene = false;
    [HideInInspector] public bool alreadyLoading = false;
    [HideInInspector] public bool stopTimer = false;
    [HideInInspector] public bool isPaused = false;

    [HideInInspector] public float minutes;
    [HideInInspector] public float seconds;
    CameraMovement playerCamera;
    private void Awake()
    {
        playerCamera = FindObjectOfType<CameraMovement>();
        GetComponent<Canvas>().worldCamera = playerCamera.overlay;
    }
    private void Update()
    {
        if (!stopTimer)
        {
            seconds += Time.deltaTime;

            if (seconds > 59)
            {
                minutes += 1;
                seconds = 0;
            }

            levelTimer.text = levelTimerCopy.text = $"{minutes:00}.{seconds:00.00}";
        }

        if (Input.GetButtonDown("Pause"))
        {
            Pause();
        }

        if (playerCamera.transform.position.y < -300)
        {
            ResetLevel();
        }
    }
    public void Pause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            pauseMenu.SetActive(false);
            settingsMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }
    public void OpenSettings(bool open)
    {
        pauseMenu.SetActive(!open);
        settingsMenu.SetActive(open);

        mouseSensitivity.value = PlayerPrefs.GetFloat("MouseSensitivity", 500);
        sfxVolume.value = PlayerPrefs.GetFloat("SfxVolume", .3f);
        musicVolume.value = PlayerPrefs.GetFloat("MusicVolume", .6f);
        usePostProcessing.isOn = PlayerPrefs.GetInt("EnablePostProcessing", 1) == 1;
        SaveSettings();
    }
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity.value);
        PlayerPrefs.SetFloat("SfxVolume", sfxVolume.value);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume.value);
        PlayerPrefs.SetInt("EnablePostProcessing", usePostProcessing.isOn ? 1 : 0);
        ApplySettings();
    }
    public void ApplySettings()
    {
        playerCamera.sensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 500);
        AudioManager.instance.ChangeSFXVolume();
        AudioManager.instance.ChangeMusicVolume();
        FindObjectOfType<Volume>().gameObject.SetActive(
            PlayerPrefs.GetInt("EnablePostProcessing", 1) == 1
        );
    }

    void ResetLevel()
    {

    }
}
