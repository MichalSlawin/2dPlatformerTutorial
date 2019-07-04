using UnityEngine;
using System;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private const float MOVE_DISTANCE = 3.5f;
    private const float JUMP_HEIGHT = 5.5f;
    private const int RUN_MULTIPLIER_BONUS = 2;
    private const int STAMINA_MAX = 1000;
    private const int STAMINA_MIN = 0;
    private const int STAMINA_RUN_PENALTY = -2;
    private const int STAMINA_JUMP_PENALTY = -50;
    private const int STAMINA_WAIT_BONUS = 2;
    private const int STAMINA_WALK_BONUS = 1;
    private const int RUN_MIN_STAMINA = 100;
    private const int JUMP_MIN_STAMINA = 100;
    private const int CROUCH_MOVE_DIVIDER = 2;
    private const float MOVE_LIMIT = 1f;
    private const int GEMS_NUM = 10;

    private const KeyCode JUMP_KEY = KeyCode.W;
    private const KeyCode LEFT_KEY = KeyCode.A;
    private const KeyCode RIGHT_KEY = KeyCode.D;
    private const KeyCode CROUCH_KEY = KeyCode.S;
    private const KeyCode SPRINT_KEY = KeyCode.LeftShift;

    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask ground;

    private int stamina = STAMINA_MAX;
    [SerializeField] Text staminaText;
    [SerializeField] private float moveDistance = MOVE_DISTANCE;
    [SerializeField] private float jumpHeight = JUMP_HEIGHT;

    private enum State {idling, running, jumping, crouching, falling, hurt}
    private State state = State.idling;

    private int gemsCollected = 0;
    [SerializeField] private int gemsLeft = GEMS_NUM;
    [SerializeField] private Text gemsText;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        gemsText.text = gemsLeft.ToString();
        staminaText.text = stamina.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(state != State.hurt)
        {
            ManageInput();
        }
        
        SwitchStateVelocity();

        animator.SetInteger("state", (int)state);

        staminaText.text = stamina.ToString();

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == "Collectable")
        {
            Destroy(collider.gameObject);
            gemsCollected += 1;
            gemsLeft -= 1;
            gemsText.text = gemsLeft.ToString();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            if(state == State.falling)
            {
                Destroy(collision.gameObject);
                Jump();
            }
            else
            {
                state = State.hurt;

                if(collision.gameObject.transform.position.x > transform.position.x)
                {
                    Move(-MOVE_DISTANCE, STAMINA_JUMP_PENALTY, true);
                }
                else
                {
                    Move(MOVE_DISTANCE, STAMINA_JUMP_PENALTY, true);
                }
            }
        }
    }

    private void ManageInput()
    {
        float hDirection = Input.GetAxis("Horizontal");
        float vDirection = Input.GetAxis("Vertical");

        if (boxCollider.IsTouchingLayers() && !Input.GetKey(JUMP_KEY) && hDirection < 0 && (!Input.GetKey(SPRINT_KEY) || (Input.GetKey(SPRINT_KEY) && stamina <= RUN_MIN_STAMINA)))
        {
            Move(-moveDistance, STAMINA_WALK_BONUS, false);
            transform.localScale = new Vector2(-1, 1);
        }
        else if (state != State.crouching && boxCollider.IsTouchingLayers() && !Input.GetKey(JUMP_KEY) && hDirection < 0 && Input.GetKey(SPRINT_KEY) && stamina > RUN_MIN_STAMINA)
        {
            Move(-moveDistance * RUN_MULTIPLIER_BONUS, STAMINA_RUN_PENALTY, true);
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
            if(stamina == STAMINA_MAX-1)
            {
                stamina += STAMINA_WAIT_BONUS-1;
            }
            else
            {
                stamina += STAMINA_WAIT_BONUS;
            }
        }

        if (Input.GetButtonDown("Jump") && boxCollider.IsTouchingLayers(ground))
        {
            Jump();
        }
        if (Input.GetKey(CROUCH_KEY) && boxCollider.IsTouchingLayers(ground))
        {
            moveDistance = MOVE_DISTANCE / CROUCH_MOVE_DIVIDER;
            state = State.crouching;
        }
        if (Input.GetKeyUp(CROUCH_KEY))
        {
            state = State.idling;
        }
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
        else if(state == State.hurt)
        {
            if(Math.Abs(rb.velocity.x) < MOVE_LIMIT)
            {
                state = State.idling;
                moveDistance = MOVE_DISTANCE;
            }
        }
        else if(Math.Abs(rb.velocity.x) > MOVE_LIMIT)
        {
            state = State.running;
        }
        else
        {
            state = State.idling;
        }
    }

    private void Move (float xValue, int staminaValue, bool penalty)
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
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            stamina += STAMINA_JUMP_PENALTY;
            state = State.jumping;
        }
    }

}
