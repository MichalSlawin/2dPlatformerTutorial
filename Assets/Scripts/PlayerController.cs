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
    private const KeyCode SPRINT_KEY = KeyCode.LeftShift;

    public Rigidbody2D rb;
    private Timer jumpTime;
    private bool jumped = false;
    private int stamina = 1000;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetKey(JUMP_KEY) && Input.GetKey(LEFT_KEY) && (!Input.GetKey(SPRINT_KEY) || (Input.GetKey(SPRINT_KEY) && stamina <= RUN_MIN_STAMINA)))
        {
            move(-1 * MOVE_DISTANCE, STAMINA_WALK_BONUS, false);
            transform.localScale = new Vector2(-1, 1);
        }
        else if (!Input.GetKey(JUMP_KEY) && Input.GetKey(LEFT_KEY) && Input.GetKey(SPRINT_KEY) && stamina > RUN_MIN_STAMINA)
        {
            //Debug.Log("sprint");
            move(-1 * MOVE_DISTANCE * RUN_MULTIPLIER_BONUS, STAMINA_RUN_PENALTY, true);
            transform.localScale = new Vector2(-1, 1);
        }
        else if (!Input.GetKey(JUMP_KEY) && Input.GetKey(RIGHT_KEY) && (!Input.GetKey(SPRINT_KEY) || (Input.GetKey(SPRINT_KEY) && stamina <= RUN_MIN_STAMINA)))
        {
            move(MOVE_DISTANCE, STAMINA_WALK_BONUS, false);
            transform.localScale = new Vector2(1, 1);
        }
        else if (!Input.GetKey(JUMP_KEY) && Input.GetKey(RIGHT_KEY) && Input.GetKey(SPRINT_KEY) && stamina > RUN_MIN_STAMINA)
        {
            //Debug.Log("sprint");
            move(MOVE_DISTANCE * RUN_MULTIPLIER_BONUS, STAMINA_RUN_PENALTY, true);
            transform.localScale = new Vector2(1, 1);
        }
        else
        {
            if(stamina < STAMINA_MAX)
            {
                stamina += STAMINA_WAIT_BONUS;
            }
        }

        if (Input.GetKeyDown(JUMP_KEY))
        {
            jump();
        }
    }

    private void move (int xValue, int staminaValue, bool penalty)
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
