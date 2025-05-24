using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject[] menus;
    int activeMenuIndex = 0;

    [Header("Settings UI Elements")]
    public Slider mouseSensitivity;
    public Slider musicVolume;
    public Slider sfxVolume;
    public Toggle enablePostProcessing;

    private void Awake()
    {
        mouseSensitivity.value = PlayerPrefs.GetFloat("MouseSensitivity", 500);
        musicVolume.value = PlayerPrefs.GetFloat("MusicVolume", 50);
        sfxVolume.value = PlayerPrefs.GetFloat("SfxVolume", 50);

        enablePostProcessing.isOn = PlayerPrefs.GetInt("EnablePostProcessing", 1) == 1;
    }

    public void ChangeMenu(int index)
    {
        menus[activeMenuIndex].SetActive(false);
        activeMenuIndex = index;
        menus[activeMenuIndex].SetActive(true);
    }

    public void LoadLevel(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity.value);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume.value);
        PlayerPrefs.SetFloat("SfxVolume", sfxVolume.value);
        PlayerPrefs.SetInt("EnablePostProcessing", enablePostProcessing.isOn ? 1 : 0);
        ApplySettings();
    }

    public void ApplySettings()
    {
        AudioManager.instance.ChangeSFXVolume();
        AudioManager.instance.ChangeMusicVolume();
        FindObjectOfType<Volume>().gameObject.SetActive(
            PlayerPrefs.GetInt("EnablePostProcessing", 1) == 1
        );
    }

    public void Quit()
    {
        Application.Quit();
    }
}
