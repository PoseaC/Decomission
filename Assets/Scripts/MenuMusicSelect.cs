using UnityEngine;
using UnityEngine.UI;

public class MenuMusicSelect : MonoBehaviour
{
    public enum ToggleType { Playlist, Shuffle}

    public ToggleType toggleType;
    public CanvasGroup canvasGroup;
    public Toggle option;
    private void Start()
    {
        switch (toggleType)
        {
            case ToggleType.Playlist:
                option.isOn = PlayerPrefs.GetInt("PlayerPlaylist", 0) == 1;
                AudioManager.instance.playerPlaylist = option.isOn;
                break;

            case ToggleType.Shuffle:
                option.isOn = PlayerPrefs.GetInt("OnShuffle", 0) == 1;
                AudioManager.instance.playerPlaylist = option.isOn;
                break;
        }

        canvasGroup.alpha = option.isOn ? 1 : 0;
    }
    public void Change()
    {
        canvasGroup.alpha = option.isOn ? 1 : 0;

        switch (toggleType)
        {
            case ToggleType.Playlist:
                PlayerPrefs.SetInt("PlayerPlaylist", option.isOn ? 1 : 0);
                AudioManager.instance.playerPlaylist = option.isOn;
                break;

            case ToggleType.Shuffle:
                PlayerPrefs.SetInt("OnShuffle", option.isOn ? 1 : 0);
                AudioManager.instance.playerPlaylist = option.isOn;
                break;
        }
    }
}
