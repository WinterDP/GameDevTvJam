using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    
    //respawn
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private CinemachineVirtualCameraBase CameraBase;

    //audio
    [SerializeField] private AudioMixer audioMixer;



    //pause n resume
    [SerializeField] private GameObject pauseMenuUI;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }


    public void respawn(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //GameObject player = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
        //CameraBase.Follow = player.transform;
    }

    public void pause(){
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void resume(){
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void MainMenu(){
        resume();
        SceneManager.LoadScene("MainMenu");
    } 

    public void quitGame(){
        Debug.Log("Saiu");
        Application.Quit();
    }

    public void setVolume(float volume){
        Debug.Log(volume);
        audioMixer.SetFloat("volume",volume);
    }

}
