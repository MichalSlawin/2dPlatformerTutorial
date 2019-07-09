using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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
    private const int GEMS_NUM = 12;
    private const int HEALTH_POINTS = 100;
    private const int ENEMY_ATTACK_DAMAGE = 20;
    private const int POWERUP_DURATION = 10;
    private const int FROGS_NUMBER = 2;

    private const string DEATH_SCENE_NAME = "DeathScene";
    private const string VICTORY_SCENE_NAME = "VictoryScene";
    private const string SECOND_SCENE_NAME = "SecondScene";

    private const KeyCode UP_KEY = KeyCode.W;
    private const KeyCode LEFT_KEY = KeyCode.A;
    private const KeyCode RIGHT_KEY = KeyCode.D;
    private const KeyCode CROUCH_KEY = KeyCode.S;
    private const KeyCode SPRINT_KEY = KeyCode.LeftShift;
    private const KeyCode CHANGE_CHARACTER = KeyCode.R;

    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask ladder;

    [SerializeField] private AudioSource footstepSound;
    [SerializeField] private AudioSource gemPickingSound;
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource hurtSound;

    private int stamina = STAMINA_MAX;
    [SerializeField] Text staminaText;
    [SerializeField] private float moveDistance = MOVE_DISTANCE;
    [SerializeField] private float jumpHeight = JUMP_HEIGHT;

    private int healthPoints = HEALTH_POINTS;
    [SerializeField] private Text healthText;

    private enum State {idling, running, jumping, crouching, falling, hurt, frozen, climbing}
    private State state = State.idling;

    private int gemsCollected = 0;
    [SerializeField] private int gemsLeft = GEMS_NUM;
    [SerializeField] private Text gemsText;

    [SerializeField] private int frogsLeft = FROGS_NUMBER;

    private CharacterChangeController characterChangeController;
    private CameraController cameraController;

    private bool turnedRight;
    private bool canClimb = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        gemsText.text = gemsLeft.ToString();
        staminaText.text = stamina.ToString();
        healthText.text = healthPoints.ToString();

        characterChangeController = GameObject.FindObjectOfType(typeof(CharacterChangeController)) as CharacterChangeController;
        cameraController = GameObject.FindObjectOfType(typeof(CameraController)) as CameraController;

        if(transform.localScale.x == 1)
        {
            turnedRight = true;
        }
        else
        {
            turnedRight = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.idling || state == State.frozen)
        {
            ManageChangeCharacter();
        }

        if (state != State.hurt && state != State.frozen)
        {
            ManageInput();
        }
        
        if(state != State.frozen)
        {
            SwitchStateVelocity();
        }

        animator.SetInteger("state", (int)state);

        staminaText.text = stamina.ToString();

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == "Collectable")
        {
            gemPickingSound.Play();
            Destroy(collider.gameObject);
            gemsCollected += 1;
            gemsLeft -= 1;
            gemsText.text = gemsLeft.ToString();
        }

        if(collider.tag == "Powerup")
        {
            gemPickingSound.Play();
            Destroy(collider.gameObject);
            GetComponent<SpriteRenderer>().color = Color.yellow;
            moveDistance = MOVE_DISTANCE * RUN_MULTIPLIER_BONUS;
            StartCoroutine(CancelPowerup());
        }

        if(collider.tag == "Finish" && gemsLeft <= 0)
        {
            if(frogsLeft < 2)
            {
                SceneManager.LoadScene(VICTORY_SCENE_NAME);
            }
            else
            {
                SceneManager.LoadScene(SECOND_SCENE_NAME);
            }
        }

        if(collider.tag == "Ladder")
        {
            canClimb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ladder")
        {
            canClimb = false;
            state = State.idling;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if(state == State.falling)
            {
                enemy.JupmedOn();
                Jump();
                if(enemy.GetType() == typeof(Frog))
                {
                    frogsLeft--;
                }
            }
            else
            {
                if(state != State.frozen)
                {
                    state = State.hurt;
                    healthPoints -= ENEMY_ATTACK_DAMAGE;
                    healthText.text = healthPoints.ToString();
                }

                if(healthPoints <= 0)
                {
                    SceneManager.LoadScene(DEATH_SCENE_NAME);
                }

                if(collision.gameObject.transform.position.x > transform.position.x)
                {
                    if(state != State.frozen)
                    {
                        Move(-MOVE_DISTANCE, STAMINA_JUMP_PENALTY, true);
                    }
                }
                else
                {
                    if (state != State.frozen)
                    {
                        Move(MOVE_DISTANCE, STAMINA_JUMP_PENALTY, true);
                    }
                }
            }
        }
    }

    private void ManageChangeCharacter()
    {
        if (Input.GetKeyDown(CHANGE_CHARACTER))
        {
            characterChangeController.SwitchCharacter();

            if (state == State.frozen)
            {
                state = State.idling;
            }
            else
            {
                state = State.frozen;
            }
        }
    }

    private void ManageInput()
    {
        float hDirection = Input.GetAxis("Horizontal");
        float vDirection = Input.GetAxis("Vertical");

        if ((boxCollider.IsTouchingLayers(ground) || boxCollider.IsTouchingLayers(ladder)) && hDirection < 0 && (!Input.GetKey(SPRINT_KEY) || (Input.GetKey(SPRINT_KEY) && stamina <= RUN_MIN_STAMINA)))
        {
            turnCharacter(false);
            Move(-moveDistance, STAMINA_WALK_BONUS, false);
        }
        else if (state != State.crouching && boxCollider.IsTouchingLayers(ground) && hDirection < 0 && Input.GetKey(SPRINT_KEY) && stamina > RUN_MIN_STAMINA)
        {
            turnCharacter(false);
            Move(-moveDistance * RUN_MULTIPLIER_BONUS, STAMINA_RUN_PENALTY, true);
        }
        else if ((boxCollider.IsTouchingLayers(ground) || boxCollider.IsTouchingLayers(ladder)) && hDirection > 0 && (!Input.GetKey(SPRINT_KEY) || (Input.GetKey(SPRINT_KEY) && stamina <= RUN_MIN_STAMINA)))
        {
            turnCharacter(true);
            Move(moveDistance, STAMINA_WALK_BONUS, false);
        }
        else if (state != State.crouching && boxCollider.IsTouchingLayers(ground) && hDirection > 0 && Input.GetKey(SPRINT_KEY) && stamina > RUN_MIN_STAMINA)
        {
            turnCharacter(true);
            Move(moveDistance * RUN_MULTIPLIER_BONUS, STAMINA_RUN_PENALTY, true);
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
            moveDistance = MOVE_DISTANCE;
        }

        if(canClimb && Input.GetKey(UP_KEY))
        {
            state = State.climbing;
            transform.position = new Vector2(transform.position.x, transform.position.y + 0.2f);
        }
    }

    private void turnCharacter(bool right)
    {
        if(right)
        {
            if(!turnedRight)
            {
                transform.localScale = new Vector2(1, 1);
                
                //cameraController.swapOffsetX(true);

                turnedRight = true;
            }
        }
        else
        {
            if(turnedRight)
            {
                transform.localScale = new Vector2(-1, 1);
                //cameraController.swapOffsetX(false);

                turnedRight = false;
            }
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
        else if(state == State.climbing || state == State.crouching)
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

    private IEnumerator CancelPowerup()
    {
        yield return new WaitForSeconds(POWERUP_DURATION);
        moveDistance = MOVE_DISTANCE;
        GetComponent<SpriteRenderer>().color = Color.white;
    }


    private void PlayFootsteps()
    {
        footstepSound.Play();
    }

    private void PlayJump()
    {
        jumpSound.Play();
    }

    private void PlayHurt()
    {
        hurtSound.Play();
    }
}
