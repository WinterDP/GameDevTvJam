using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D controller;

    //relacionado a movimento
    [Range(0, .5f)] [SerializeField] private float m_MovementSmoothing = .05f; // How much to smooth out the movement
    private Vector3 m_Velocity = Vector3.zero;
    public float speedMovement;
    private float moveInput;

    //relacionado a pulo
    private bool isGrounded; //verifica se o player esta no chao
    public Transform feetpos;
    public float checkRadius;
    public LayerMask whatIsGround; //define o que e chao
    public float jumpForce;
    private float jumpTimeCounter;
    public float jumpTime;
    private bool isJumping;
    private bool startJump;
    

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(feetpos.position, checkRadius, whatIsGround);

        //Se soltar a tecla o personagem comeca a cair
        if(Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;    
        }
          //faz o personagem pular
        if(isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            controller.velocity = Vector2.up * jumpForce;
        }

        

    }


    void FixedUpdate() {
        // atribiu o lado que o jogador deseja andar de acordo com a tecla apertada -1|0|1
        moveInput = Input.GetAxisRaw("Horizontal");
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(moveInput * speedMovement, controller.velocity.y);
        // And then smoothing it out and applying it to the character
        controller.velocity = Vector3.SmoothDamp(controller.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
    
        
      

        //faz o personagem pular mais alto se segurar o pulo
        if(Input.GetKey(KeyCode.Space) && isJumping)
        {
            if(jumpTimeCounter>0)
            {
                controller.velocity = Vector2.up * jumpForce;
                jumpTimeCounter-=Time.deltaTime;
            }else{
                isJumping = false;
            }
            
        }

    }
}
