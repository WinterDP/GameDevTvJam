using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    private Rigidbody2D controller;
    private BoxCollider2D playerBoxCollider;
    private CircleCollider2D playerCircleCollider;



    //movement
    [Range(0, .5f)] [SerializeField] private float m_MovementSmoothing = .05f; // How much to smooth out the movement
    [SerializeField] private float speedMovement;

    private Vector3 m_Velocity = Vector3.zero;
    private float moveInput;

    //MirroringPlayer
    private bool isFacingRight = true;

    //jumping
    
    [SerializeField] private Transform groundCheck; //objeto que verifica onde esta o pe do personagem
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask whatIsGround; //define o que e chao
    [SerializeField] private float jumpForce;
    [SerializeField] private float movementForceInAir;
    [Range(0, .5f)] [SerializeField] private float m_MovementSmoothinginAir = .05f;
    [SerializeField] private float resistAir = 0.95f;
    [SerializeField] private float variableJumHeightMultiplier = 0.5f;
    [SerializeField] private float jumpTime;
    [SerializeField] private int amountOfJumps = 1;


    private bool isGrounded; //verifica se o player esta no chao
    private float jumpTimeCounter;
    private bool isJumping;
    private bool startJump;
    private bool canJump;
    
    private int amountOfJumpsLeft;
    

    //Animations
    private Animator animator;
    private bool isWalking;
    
    //WallSlide
    
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float totalWallSlidingSpeed;
    [SerializeField] private float increaseWallSliperry;
    [SerializeField]  private float limitWallslidingSpeed = 25f;

    private bool isTouchingWall; //verifica se esta tocando a parede
    private bool isWallSliding;
    private float wallSlidingSpeed;
    
    //wall jump
    [SerializeField] private Vector2 wallHopDirection;
    [SerializeField] private Vector2 wallJumpDirection;
    [SerializeField] private float wallHopForce;
    [SerializeField] private float wallJumpForce;

    private int facingDirection = 1; //-1 left | 1 right

    //killPlayer
    [SerializeField] private float lethalVelocity;
    [SerializeField] private float deathAnimation = 2f;
    [SerializeField] private CinemachineVirtualCameraBase CameraBase;
    

    private bool isDead;
    private bool isTouchingEnemy;

    //oneWayPlatform
    private bool isOnOneWayPlatform;
    private GameObject currentOnWayPlatform;

    //pause game
    private bool gameIsPaused;
    


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        controller = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerBoxCollider = GetComponent<BoxCollider2D>();
        playerCircleCollider = GetComponent<CircleCollider2D>();
        amountOfJumpsLeft = amountOfJumps;
        wallSlidingSpeed = totalWallSlidingSpeed;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
        gameIsPaused = false;

    }

    void Start()
    {
        transform.position = LevelManager.instance.lastCheckpointPosition;
    }



    // Update is called once per frame
    void Update()
    {
        checkIfIsDead();
        checkInput();
        checkMovementDirection();
        updateAnimations();
        checkSurroundings();
        checkIfCanJump();
        checkIfwallIsSliding();
    }

    //
    private void checkSurroundings(){
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right,wallCheckDistance,whatIsGround);
        
    }


    void FixedUpdate() {
        applyMovement();
    }

    public void updateAnimations(){
        animator.SetBool("isDead",isDead);
        animator.SetBool("isWalking",isWalking);
        animator.SetBool("isJumping",isJumping);
        animator.SetBool("isWallSliding",isWallSliding);
        
    }

    private void checkInput(){
        if(!isDead){
            // atribiu o lado que o jogador deseja andar de acordo com a tecla apertada -1|0|1
            moveInput = Input.GetAxisRaw("Horizontal");
            if(moveInput !=0){
                isWalking = true;
            }


            // verifica se o personagem esta no chao e pode pular
            if(Input.GetKeyDown(KeyCode.Space))
            {
                if(canJump){
                    jump();
                }

            }
            //Se soltar a tecla o personagem comeca a cair
            if(Input.GetKeyUp(KeyCode.Space) && isJumping)
            {
                controller.velocity = new Vector2(controller.velocity.x, controller.velocity.y * variableJumHeightMultiplier);
                isJumping = false;    
            }

            if(Input.GetKeyDown(KeyCode.S)||Input.GetKeyDown(KeyCode.DownArrow)){
                if(currentOnWayPlatform != null){
                    StartCoroutine(fallOfOneWayPlatform());
                }
            }

            if(Input.GetKeyDown(KeyCode.Escape)){
                pauseGame();
            }
        }


    }

    //faz o personagem pular
    private void checkIfCanJump(){
        if((isGrounded && controller.velocity.y <=0) || isWallSliding){
            amountOfJumpsLeft = amountOfJumps;
        }
        if(amountOfJumpsLeft<=0 || (amountOfJumpsLeft == amountOfJumps && !isGrounded && !isWallSliding)){
            canJump = false;
        }else{
            canJump = true;
        }
    }

    private void checkIfwallIsSliding(){
        if(isTouchingWall && !isGrounded && controller.velocity.y<0){
            Debug.Log("aaaa");
            isWallSliding = true;
        }else{
            isWallSliding = false;
            wallSlidingSpeed = totalWallSlidingSpeed;
        }

    }

    private void applyMovement(){

        if(isGrounded){
            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(moveInput * speedMovement, controller.velocity.y);
            // And then smoothing it out and applying it to the character
            controller.velocity = Vector3.SmoothDamp(controller.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
            
        } else if(!isGrounded && !isWallSliding && moveInput != 0){
            Vector2 forceToAdd = new Vector2(movementForceInAir * moveInput, 0);
            controller.AddForce(forceToAdd);
            
            if(Mathf.Abs(controller.velocity.x)>speedMovement){
                // Move the character by finding the target velocity
                Vector3 targetVelocity = new Vector2(moveInput * speedMovement, controller.velocity.y);
                // And then smoothing it out and applying it to the character
                controller.velocity  = Vector3.SmoothDamp(controller.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothinginAir);
            }

            

        } else if(!isGrounded && !isWallSliding && moveInput==0){
            controller.velocity = new Vector2(controller.velocity.x*resistAir,controller.velocity.y);
            
        }
        

        if(isWallSliding){
            if(controller.velocity.y < -wallSlidingSpeed){
                
                controller.velocity = new Vector2(controller.velocity.x,-wallSlidingSpeed);
            }
            if(wallSlidingSpeed<=limitWallslidingSpeed){
                
                wallSlidingSpeed = wallSlidingSpeed+increaseWallSliperry; // atualiza a velocidade que o personagem cai
            }
            

        }
        
    }

    private void checkMovementDirection(){
        if(isFacingRight && moveInput < 0){
            flip();
        }else if(!isFacingRight && moveInput>0){
            flip();
        }

        if(moveInput ==0 || !isGrounded){
            isWalking = false;
        }
    }

    public void flip(){
        if(!isWallSliding){
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f,180.0f,0.0f);
        }
        
    }

    public void jump()
    {
        if(canJump && !isWallSliding){
            SoundManagerScript.instance.PlaySound("jump");
            isJumping = true;
            jumpTimeCounter = jumpTime;
            controller.velocity = new Vector2(controller.velocity.x,jumpForce);
            amountOfJumpsLeft--;
        }else if((isWallSliding || isTouchingWall) && (moveInput != facingDirection) && canJump){
            SoundManagerScript.instance.PlaySound("jump");
            isJumping = true;
            isWallSliding = false;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce*wallJumpDirection.x*-facingDirection,wallSlidingSpeed);
            controller.AddForce(forceToAdd, ForceMode2D.Impulse);
        }/*else if(isWallSliding && moveInput == 0 && canJump){ //wall Hop
            isJumping = true;
            isWallSliding = false;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallHopForce*wallHopDirection.x*-facingDirection,wallHopForce*wallHopDirection.y);
            controller.AddForce(forceToAdd, ForceMode2D.Impulse);
        }*/

    }

    public void checkIfIsDead(){
        if((isGrounded && (controller.velocity.y < -lethalVelocity)) || controller.velocity.y < (-lethalVelocity - 10f)){
            isDead = true;
            animator.SetBool("isDead",isDead);
            
            StartCoroutine(killPlayer());
        }else if(isTouchingEnemy){
            isDead = true;
            StartCoroutine(killPlayer());
        }else{
            isDead = false;
        }
    }

    public IEnumerator killPlayer(){

        isDead = true;
        SoundManagerScript.instance.PlaySound("death");
        CameraBase.Follow = null;
        playerBoxCollider.enabled = false;
        playerCircleCollider.enabled = false;
        controller.velocity = new Vector2(0,-0.5f);
        controller.isKinematic = true;

        yield return new WaitForSeconds(deathAnimation);

        isDead = false;
        Destroy(gameObject);
        LevelManager.instance.respawn();

        
    }

    
    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.CompareTag("Enemy")){
            isTouchingEnemy = true;
        }else{
            isTouchingEnemy = false;
        }

        if(collision.gameObject.CompareTag("OneWayPlatformer")){
            currentOnWayPlatform = collision.gameObject;
        }

        if(collision.gameObject.CompareTag("NextLevel")){
            LevelManager.instance.NextLevel();
        }


    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if(collision.gameObject.CompareTag("NextLevel")){
            LevelManager.instance.NextLevel();
        }


    }

    private void OnCollisionExit2D(Collision2D collision) {
        if(collision.gameObject.CompareTag("OneWayPlatformer")){
            currentOnWayPlatform = null;
        }
    }

    private IEnumerator fallOfOneWayPlatform(){
        float waitingTime = 0.5f;
        PlatformEffector2D plataformEfector = currentOnWayPlatform.GetComponent<PlatformEffector2D>();
        
            plataformEfector.rotationalOffset = 180f;
            yield return new WaitForSeconds(waitingTime);
            plataformEfector.rotationalOffset = 0f;

    }

    private void pauseGame(){
        if(gameIsPaused){
            gameIsPaused = false;
            LevelManager.instance.resume();
        }else{
            gameIsPaused = true;            
            LevelManager.instance.pause();
        }
    }
}
