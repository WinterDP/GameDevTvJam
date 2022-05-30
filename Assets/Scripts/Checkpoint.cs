using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isActive;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Update() {
        updateAnimations();
        
    }

    public void updateAnimations(){
        animator.SetBool("isActive",isActive);
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Player")){
            SoundManagerScript.instance.PlaySound("checkpoint");
            isActive = true;

            Debug.Log("novo checkpoint");
            LevelManager.instance.lastCheckpointPosition = transform.position;
        }
    }
}
