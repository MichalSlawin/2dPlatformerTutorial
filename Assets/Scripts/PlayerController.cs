using System.Collections;
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
    private const int JUMP_WAIT = 2000;

    private const KeyCode JUMP_KEY = KeyCode.W;
    private const KeyCode LEFT_KEY = KeyCode.A;
    private const KeyCode RIGHT_KEY = KeyCode.D;
    private const KeyCode CROUCH_KEY = KeyCode.S;
    private const KeyCode SPRINT_KEY = KeyCode.LeftShift;

    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D boxCollider;

    private Timer jumpTime;
    private bool jumped = false;
    private int stamina = 1000;
    private int moveDistance = MOVE_DISTANCE;

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

        if (!Input.GetKey(JUMP_KEY) && hDirection < 0 && (!Input.GetKey(SPRINT_KEY) || (Input.GetKey(SPRINT_KEY) && stamina <= RUN_MIN_STAMINA)))
        {
            move(-1 * moveDistance, STAMINA_WALK_BONUS, false);
            transform.localScale = new Vector2(-1, 1);
        }
        else if (!Input.GetKey(JUMP_KEY) && hDirection < 0 && Input.GetKey(SPRINT_KEY) && stamina > RUN_MIN_STAMINA)
        {
            //Debug.Log("sprint");
            move(-1 * moveDistance * RUN_MULTIPLIER_BONUS, STAMINA_RUN_PENALTY, true);
            transform.localScale = new Vector2(-1, 1);
        }
        else if (!Input.GetKey(JUMP_KEY) && hDirection > 0 && (!Input.GetKey(SPRINT_KEY) || (Input.GetKey(SPRINT_KEY) && stamina <= RUN_MIN_STAMINA)))
        {
            move(moveDistance, STAMINA_WALK_BONUS, false);
            transform.localScale = new Vector2(1, 1);
        }
        else if (!Input.GetKey(JUMP_KEY) && hDirection > 0 && Input.GetKey(SPRINT_KEY) && stamina > RUN_MIN_STAMINA)
        {
            //Debug.Log("sprint");
            move(moveDistance * RUN_MULTIPLIER_BONUS, STAMINA_RUN_PENALTY, true);
            transform.localScale = new Vector2(1, 1);
        }
        else
        {
            animator.SetBool("crouching", false);
            animator.SetBool("running", false);
            if (stamina < STAMINA_MAX)
            {
                stamina += STAMINA_WAIT_BONUS;
            }
        }

        if (vDirection > 0)
        {
            jump();
        }
        if (Input.GetKey(CROUCH_KEY))
        {
            moveDistance = MOVE_DISTANCE / 2;
            animator.SetBool("crouching", true);
        }
        if(Input.GetKeyUp(CROUCH_KEY))
        {
            moveDistance = MOVE_DISTANCE;
        }
    }

    private void move (int xValue, int staminaValue, bool penalty)
    {
        animator.SetBool("crouching", false);
        animator.SetBool("running", true);
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

    private void jump()
    {
        if(!this.jumped && stamina >= JUMP_MIN_STAMINA)
        {
            rb.velocity = new Vector2(rb.velocity.x, JUMP_HEIGHT);
            this.jumped = true;

            SetTimer();

            stamina += STAMINA_JUMP_PENALTY;
        }
    }

    private void SetTimer()
    {
        // Create a timer with a two second interval.
        this.jumpTime = new System.Timers.Timer(JUMP_WAIT);
        // Hook up the Elapsed event for the timer. 
        this.jumpTime.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        this.jumpTime.AutoReset = false;
        this.jumpTime.Enabled = true;
    }

    private void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        this.jumped = false;  
    }

}
