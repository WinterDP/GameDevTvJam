using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance; 

    
    //respawn
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private CinemachineVirtualCameraBase CameraBase;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }


    public void Respawn(){
        GameObject player = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
        CameraBase.Follow = player.transform;
    }

}
