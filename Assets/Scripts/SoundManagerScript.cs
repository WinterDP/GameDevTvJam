using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public static SoundManagerScript instance;

    public static AudioClip playerJumpSound, playerDeathSound, playerCheckpointSound;

    private static AudioSource audioSource;

    private void Awake() {
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(instance);
        }else{
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        playerJumpSound = Resources.Load<AudioClip> ("playerJumpSound");
        playerDeathSound = Resources.Load<AudioClip> ("playerDeathSound");
        playerCheckpointSound = Resources.Load<AudioClip> ("playerCheckpointSound");

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound(string clip){
        switch(clip){
            case "jump":
                audioSource.PlayOneShot(playerJumpSound);
                break;
            case "death":
                audioSource.PlayOneShot(playerDeathSound);
                break;
            case "checkpoint":
                audioSource.PlayOneShot(playerCheckpointSound);
                break;  
        }

    }
}
