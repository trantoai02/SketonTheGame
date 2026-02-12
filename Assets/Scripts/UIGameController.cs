using UnityEngine;
using UnityEngine.UI;

public class UIGameController : MonoBehaviour
{
    [Header("Player Menu")]
    public Slider musicSlider;
    public Slider sfxSlider;

    public void MusicVolume()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.SetMusicVolume(musicSlider.value);
    }

    public void SFXVolume()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.SetSFXVolume(sfxSlider.value);
    }
}
