using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D controller;

    //relacionado a movimento
    [Range(0, .5f)] [SerializeField] private float m_MovementSmoothing = .05f; // How much to smooth out the movement
    private Vector3 m_Velocity = Vector3.zero;
    [SerializeField] private float speedMovement;
    private float moveInput;

    //relacionado ao espelhamento do sprite na mudanca de direcao
    private bool isFacingRight = true;

    //relacionado a pulo
    private bool isGrounded; //verifica se o player esta no chao
    [SerializeField] private Transform feetpos; //objeto que verifica onde esta o pe do personagem
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask whatIsGround; //define o que e chao
    [SerializeField] private float jumpForce;
    private float jumpTimeCounter;
    [SerializeField] private float jumpTime;
    private bool isJumping;
    private bool startJump;
    private bool canJump;
    [SerializeField] private int amountOfJumps = 1;
    private int amountOfJumpsLeft;
    [SerializeField] private float movementForceInAir;
    [Range(0, .5f)] [SerializeField] private float m_MovementSmoothinginAir = .05f;
    [SerializeField] private float resistAir = 0.95f;
    [SerializeField] private float variableJumHeightMultiplier = 0.5f;

    //Animacoes
    private Animator animator;
    private bool isWalking;
    
    //Deslizar na parede
    private bool isTouchingWall; //verifica se esta tocando a parede
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private Transform wallCheck;
    private bool isWallSliding;
    private float wallSlidingSpeed;
    [SerializeField] private float totalWallSlidingSpeed;
    [SerializeField] private float increaseWallSliperry;

    //wall jump
    [SerializeField] private Vector2 wallHopDirection;
    [SerializeField] private Vector2 wallJumpDirection;

    [SerializeField] private float wallHopForce;
    [SerializeField] private float wallJumpForce;
    private int facingDirection = 1; //-1 left | 1 right



    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        amountOfJumpsLeft = amountOfJumps;
        wallSlidingSpeed = totalWallSlidingSpeed;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();

    }

    // Update is called once per frame
    void Update()
    {
        checkInput();
        checkMovementDirection();
        updateAnimations();
        checkSurroundings();
        checkIfCanJump();
        checkIfwallIsSliding();
    }

    private void checkSurroundings(){
        isGrounded = Physics2D.OverlapCircle(feetpos.position, checkRadius, whatIsGround);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right,wallCheckDistance,whatIsGround);
    }


    void FixedUpdate() {
        applyMovement();
    }

    private void updateAnimations(){
        animator.SetBool("isWalking",isWalking);
        animator.SetBool("isJumping",isJumping);
        animator.SetBool("isWallSliding",isWallSliding);
    }

    private void checkInput(){
        // atribiu o lado que o jogador deseja andar de acordo com a tecla apertada -1|0|1
        moveInput = Input.GetAxisRaw("Horizontal");

        // verifica se o personagem esta no chao e pode pular
        if(Input.GetKeyDown(KeyCode.Space))
        {
            jump();
        }
        //Se soltar a tecla o personagem comeca a cair
        if(Input.GetKeyUp(KeyCode.Space))
        {
            controller.velocity = new Vector2(controller.velocity.x, controller.velocity.y * variableJumHeightMultiplier);
            isJumping = false;    
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
            isWalking = true;
        } else if(!isGrounded && !isWallSliding && moveInput != 0){
            Vector2 forceToAdd = new Vector2(movementForceInAir * moveInput, 0);
            controller.AddForce(forceToAdd);
            
            if(Mathf.Abs(controller.velocity.x)>speedMovement){
                // Move the character by finding the target velocity
                Vector3 targetVelocity = new Vector2(moveInput * speedMovement, controller.velocity.y);
                // And then smoothing it out and applying it to the character
                controller.velocity  = Vector3.SmoothDamp(controller.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothinginAir);
            }

            isWalking = true;

        } else if(!isGrounded && !isWallSliding && moveInput==0){
            controller.velocity = new Vector2(controller.velocity.x*resistAir,controller.velocity.y);
            isWalking = true;
        }
        

        if(isWallSliding){
            if(controller.velocity.y < -wallSlidingSpeed){
                controller.velocity = new Vector2(controller.velocity.x,-wallSlidingSpeed);
            }
            wallSlidingSpeed = wallSlidingSpeed+increaseWallSliperry; // atualiza a velocidade que o personagem cai

        }
        
    }

    private void checkMovementDirection(){
        if(isFacingRight && moveInput < 0){
            flip();
        }else if(!isFacingRight && moveInput>0){
            flip();
        }

        if(controller.velocity.x ==0){
            isWalking = false;
        }
    }

    private void flip(){
        if(!isWallSliding){
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f,180.0f,0.0f);
        }
        
    }

    private void jump()
    {
        if(canJump && !isWallSliding){
            isJumping = true;
            jumpTimeCounter = jumpTime;
            controller.velocity = new Vector2(controller.velocity.x,jumpForce);
            amountOfJumpsLeft--;
        }else if(isWallSliding && moveInput == 0 && canJump){ //wall Hop
            isJumping = true;
            isWallSliding = false;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallHopForce*wallHopDirection.x*-facingDirection,wallHopForce*wallHopDirection.y);
            controller.AddForce(forceToAdd, ForceMode2D.Impulse);
        }else if((isWallSliding || isTouchingWall) && (moveInput != facingDirection) && canJump){
            isJumping = true;
            isWallSliding = false;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce*wallJumpDirection.x*moveInput,wallJumpForce*wallJumpDirection.y);
            controller.AddForce(forceToAdd, ForceMode2D.Impulse);
        }

    }



}
