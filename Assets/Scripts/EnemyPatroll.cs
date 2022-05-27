using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatroll : MonoBehaviour
{
    private Rigidbody2D controller; //variavel de acesso ao rigidbody 2d do inimigo

    [SerializeField] private float speedMovement;//velocidade de movimento do inimigo
    [SerializeField] private LayerMask whatIsGround; //Guarda o que é considerado chão na cena
    [SerializeField] private Transform groundCheck; //aponta onde tem chao para o objeto

    private bool isFacingRight = true;
    private bool isWalking = true;
    private Animator animator;

    private RaycastHit2D hit; //da a informação de onde o objeto esta batendo
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
        updateAnimations();
    }

    void FixedUpdate() {
        if(hit.collider != false){
            if(isFacingRight){
                controller.velocity = new Vector2(speedMovement, controller.velocity.y);
            }else{
                controller.velocity = new Vector2(-speedMovement, controller.velocity.y);
            }
        }else{
            flip();
        }
    }
    private void updateAnimations(){
        animator.SetBool("isWalking",isWalking);
    }

    private void checkIfIsGrounded(){
        float raycastRadius = 1f;
        hit = Physics2D.Raycast(groundCheck.position, -transform.up, raycastRadius, whatIsGround);
    }

    private void flip(){
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f,180.0f,0.0f);
    }
}
