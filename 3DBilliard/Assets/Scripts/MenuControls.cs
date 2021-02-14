using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuControls : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider volumeSlider;

    public void Start()
    {
        float volumeValue;
        audioMixer.GetFloat("Volume", out volumeValue);
        volumeSlider.SetValueWithoutNotify(Mathf.Pow(10, volumeValue / 20));
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
    }
    public void StartNewGame()
    {
        SceneManager.LoadScene("GameScene");
        GameLogic.score = 0;
        GameLogic.shots = 0;
        GameLogic.gameTime = 0;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
