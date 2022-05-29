using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject optionsMenuUI;

    public void playGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void quitGame(){
        Debug.Log("saiu");
        Application.Quit();
    }

    public void optionsMenu(){
        optionsMenuUI.SetActive(true);
    }


    public void back(){
        optionsMenuUI.SetActive(false);
    }
}
