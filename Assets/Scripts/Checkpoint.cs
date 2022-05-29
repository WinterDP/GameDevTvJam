using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Player")){
            //SoundManagerScript.instance.PlaySound("checkpoint");
            Debug.Log("novo checkpoint");
            LevelManager.instance.lastCheckpointPosition = transform.position;
        }
    }
}
