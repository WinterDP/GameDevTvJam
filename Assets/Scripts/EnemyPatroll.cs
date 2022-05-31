using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatroll : MonoBehaviour
{
    private Rigidbody2D controller; //variavel de acesso ao rigidbody 2d do inimigo

    [SerializeField] private float speedMovement;//velocidade de movimento do inimigo
    [SerializeField] private LayerMask whatIsGround; //Guarda o que é considerado chão na cena
    [SerializeField] private Transform groundCheck; //aponta onde tem chao para o objeto
    [SerializeField] private Transform wallCheck; //aponta onde tem chao para o objeto

    private bool isFacingLeft = true;
    private bool isWalking = true;
    private Animator animator;

    private RaycastHit2D hitEmpty;
    private RaycastHit2D hitWall; //da a informação de onde o objeto esta batendo
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        checkIfIsGrounded();
        checkIfIsInWall();
        updateAnimations();
    }

    void FixedUpdate() {
        if(hitEmpty.collider != false){
            if(isFacingLeft){
                controller.velocity = new Vector2(speedMovement, controller.velocity.y);
            }else{
                controller.velocity = new Vector2(-speedMovement, controller.velocity.y);
            }
        }else{
            flip();
        }

        if(hitWall.collider == true){
            flip();
        }
        
    }
    private void updateAnimations(){
        animator.SetBool("isWalking",isWalking);
    }

    private void checkIfIsGrounded(){
        float raycastRadius = 1f;
        hitEmpty = Physics2D.Raycast(groundCheck.position, -transform.up, raycastRadius, whatIsGround);
    }

    private void checkIfIsInWall(){
        float raycastRadius = .1f;
        hitWall = Physics2D.Raycast(wallCheck.position, -transform.up, raycastRadius, whatIsGround);
    }

    private void flip(){
            isFacingLeft = !isFacingLeft;
            transform.Rotate(0.0f,180.0f,0.0f);
    }
}
