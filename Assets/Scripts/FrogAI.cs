using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogAI : MonoBehaviour
{
    private const float JUMP_LENGTH = 4f;
    private const float JUMP_HEIGHT = 6f;

    [SerializeField] private float leftCap;
    [SerializeField] private float rightCap;

    private float jumpLength = JUMP_LENGTH;
    private float jumpHeight = JUMP_HEIGHT;

    private bool facingLeft = true;
    [SerializeField] private LayerMask ground;

    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D boxCollider;

    private enum State {idling, jumping, falling}
    private State state = State.idling;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.idling)
        {
            ManageMovement();
        }
        
        SwitchState();
        animator.SetInteger("state", (int)state);
    }

    private void ManageMovement()
    {
        if (facingLeft)
        {
            if (transform.position.x > leftCap)
            {
                if (boxCollider.IsTouchingLayers(ground))
                {
                    transform.localScale = new Vector2(1, 1);
                    Jump(true);
                    state = State.jumping;
                }
            }
            else
            {
                facingLeft = false;
            }
        }
        else
        {
            if (transform.position.x < rightCap)
            {
                if (boxCollider.IsTouchingLayers(ground))
                {
                    transform.localScale = new Vector2(-1, 1);
                    Jump(false);
                    state = State.jumping;
                }
            }
            else
            {
                facingLeft = true;
            }
        }
    }

    private void SwitchState()
    {
        if(state == State.jumping)
        {
            if (rb.velocity.y < Mathf.Epsilon)
            {
                state = State.falling;
            }
        }
        else if (state == State.falling)
        {
            if (boxCollider.IsTouchingLayers(ground))
            {
                state = State.idling;
            }
        }
        else
        {
            state = State.idling;
        }
    }

    private void Jump(bool left)
    {
        if(left)
        {
            rb.velocity = new Vector2(-jumpLength, jumpHeight);
        }
        else
        {
            rb.velocity = new Vector2(jumpLength, jumpHeight);
        }
    }
}
