using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject optionsMenuUI;
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject creditsMenuUI;
    [SerializeField] private AudioMixer audioMixer;

    public void playGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void quitGame(){
        Debug.Log("saiu");
        Application.Quit();
    }

    public void optionsMenu(){
        optionsMenuUI.SetActive(true);
        mainMenuUI.SetActive(false);
    }

    public void creditsMenu(){
        creditsMenuUI.SetActive(true);
        mainMenuUI.SetActive(false);
    }


    public void backFromOptions(){
        optionsMenuUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }

    public void backFromCredits(){
        creditsMenuUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }

    public void setVolume(float volume){
        Debug.Log(volume);
        audioMixer.SetFloat("volume",volume);
    }
}
