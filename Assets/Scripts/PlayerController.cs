﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System;

public class PlayerController : MonoBehaviour
{
    private const int MOVE_DISTANCE = 2;
    private const int JUMP_HEIGHT = 5;
    private const int RUN_MULTIPLIER_BONUS = 2;
    private const int STAMINA_MAX = 1000;
    private const int STAMINA_MIN = 0;
    private const int STAMINA_RUN_PENALTY = -2;
    private const int STAMINA_JUMP_PENALTY = -50;
    private const int STAMINA_WAIT_BONUS = 2;
    private const int STAMINA_WALK_BONUS = 1;
    private const int RUN_MIN_STAMINA = 100;
    private const int JUMP_MIN_STAMINA = 50;

    private const KeyCode JUMP_KEY = KeyCode.W;
    private const KeyCode LEFT_KEY = KeyCode.A;
    private const KeyCode RIGHT_KEY = KeyCode.D;
    private const KeyCode CROUCH_KEY = KeyCode.S;
    private const KeyCode SPRINT_KEY = KeyCode.LeftShift;

    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask ground;

    private int stamina = 1000;
    private int moveDistance = MOVE_DISTANCE;

    private enum State {idling, running, jumping, crouching, falling}
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
        float hDirection = Input.GetAxis("Horizontal");
        float vDirection = Input.GetAxis("Vertical");

        if (boxCollider.IsTouchingLayers() && !Input.GetKey(JUMP_KEY) && hDirection < 0 && (!Input.GetKey(SPRINT_KEY) || (Input.GetKey(SPRINT_KEY) && stamina <= RUN_MIN_STAMINA)))
        {
            Move(-1 * moveDistance, STAMINA_WALK_BONUS, false);
            transform.localScale = new Vector2(-1, 1);
        }
        else if (state != State.crouching && boxCollider.IsTouchingLayers() && !Input.GetKey(JUMP_KEY) && hDirection < 0 && Input.GetKey(SPRINT_KEY) && stamina > RUN_MIN_STAMINA)
        {
            Move(-1 * moveDistance * RUN_MULTIPLIER_BONUS, STAMINA_RUN_PENALTY, true);
            transform.localScale = new Vector2(-1, 1);
        }
        else if (boxCollider.IsTouchingLayers() && !Input.GetKey(JUMP_KEY) && hDirection > 0 && (!Input.GetKey(SPRINT_KEY) || (Input.GetKey(SPRINT_KEY) && stamina <= RUN_MIN_STAMINA)))
        {
            Move(moveDistance, STAMINA_WALK_BONUS, false);
            transform.localScale = new Vector2(1, 1);
        }
        else if (state != State.crouching && boxCollider.IsTouchingLayers() && !Input.GetKey(JUMP_KEY) && hDirection > 0 && Input.GetKey(SPRINT_KEY) && stamina > RUN_MIN_STAMINA)
        {
            Move(moveDistance * RUN_MULTIPLIER_BONUS, STAMINA_RUN_PENALTY, true);
            transform.localScale = new Vector2(1, 1);
        }
        else if (stamina < STAMINA_MAX)
        {
            stamina += STAMINA_WAIT_BONUS;
        }

        if (Input.GetButtonDown("Jump") && boxCollider.IsTouchingLayers(ground))
        {
            Jump();
        }
        if (Input.GetKey(CROUCH_KEY) && boxCollider.IsTouchingLayers(ground))
        {
            moveDistance = MOVE_DISTANCE / 2;
            state = State.crouching;
        }
        if(Input.GetKeyUp(CROUCH_KEY))
        {
            moveDistance = MOVE_DISTANCE;
            state = State.idling;
        }

        SwitchStateVelocity();
        animator.SetInteger("state", (int)state);

    }

    private void SwitchStateVelocity()
    {
        if(state == State.jumping)
        {
            if(rb.velocity.y < Mathf.Epsilon)
            {
                state = State.falling;
            }
        }
        else if(state == State.falling)
        {
            if(boxCollider.IsTouchingLayers(ground))
            {
                state = State.idling;
            }
        }
        else if(state == State.crouching)
        {

        }
        else if(Math.Abs(rb.velocity.x) > 1f)
        {
            // moving
            state = State.running;
        }
        else
        {
            state = State.idling;
        }
    }

    private void Move (int xValue, int staminaValue, bool penalty)
    {
        rb.velocity = new Vector2(xValue, rb.velocity.y);

        if(penalty)
        {
            if (stamina > STAMINA_MIN)
            {
                stamina += staminaValue;
            }
        }
        else
        {
            if (stamina < STAMINA_MAX)
            {
                stamina += staminaValue;
            }
        }
        
    }

    private void Jump()
    {
        if(stamina >= JUMP_MIN_STAMINA)
        {
            rb.velocity = new Vector2(rb.velocity.x, JUMP_HEIGHT);
            stamina += STAMINA_JUMP_PENALTY;
            state = State.jumping;
        }
    }

}
