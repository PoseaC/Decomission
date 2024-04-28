using UnityEngine;
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

    [Header("Pause Menu")]
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public Slider mouseSensitivity;
    public Slider sfxVolume;
    public Slider musicVolume;
    public Toggle shufflePlaylist;

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
        musicTitle.text = TransitionInfo.instance.songTitle;
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
    public void Settings(bool open)
    {
        pauseMenu.SetActive(!open);
        settingsMenu.SetActive(open);

        mouseSensitivity.value = PlayerPrefs.GetFloat("MouseSensitivity", 500);
        sfxVolume.value = PlayerPrefs.GetFloat("SfxVolume", .3f);
        musicVolume.value = PlayerPrefs.GetFloat("MusicVolume", .6f);
        shufflePlaylist.isOn = PlayerPrefs.GetInt("OnShuffle", 0) == 1;
    }
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity.value);
        playerCamera.sensitivity = mouseSensitivity.value;

        PlayerPrefs.SetFloat("SfxVolume", sfxVolume.value);
        AudioManager.instance.ChangeSFXVolume();

        PlayerPrefs.SetFloat("MusicVolume", musicVolume.value);
        AudioManager.instance.ChangeMusicVolume();

        PlayerPrefs.SetInt("OnShuffle", shufflePlaylist.isOn ? 1 : 0);
        AudioManager.instance.onShuffle = shufflePlaylist.isOn;
    }
    public IEnumerator LoadSceneAsync(int buildIndex)
    {
        //start loading the next scene in the background while playing the outro animation for the current stage
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(buildIndex);
        asyncLoad.allowSceneActivation = false;
        alreadyLoading = true;
        canLoadScene = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                //when the next scene is loaded we save all the info we need about the player to load it in the next scene giving the impression of a smooth transition
                TransitionInfo.instance.cameraRotationX = playerCamera.xRotation;
                TransitionInfo.instance.cameraRotationY = playerCamera.yRotation;
                TransitionInfo.instance.movementState = PlayerMovementManager.instance.movementState;

                Vector3 velocity = PlayerMovementManager.instance.playerBody.velocity;
                TransitionInfo.instance.velocity = new Vector3(0, velocity.y, velocity.z);

                //when the stage director finished the outro of the previous scene we can load the next one
                if (canLoadScene)
                    asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }

    }
}
