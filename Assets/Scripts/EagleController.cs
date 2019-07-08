using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleController : MonoBehaviour
{
    private const KeyCode CHANGE_CHARACTER = KeyCode.R;
    private const float MOVE_LENGTH = 1f;
    private const float MOVE_TIME = 0.5f;

    private enum State {idling, moving, frozen}
    private State state = State.frozen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.idling || state == State.frozen)
        {
            ManageChangeCharacter();
        }

        if(state != State.frozen)
        {
            ManageInput();
        }
    }

    private void ManageChangeCharacter()
    {
        if (Input.GetKeyDown(CHANGE_CHARACTER))
        {
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

        if(hDirection < 0)
        {
            iTween.MoveUpdate(this.gameObject, iTween.Hash("x", transform.position.x - MOVE_LENGTH, "y", transform.position.y, "time", MOVE_TIME));
        }
        else if(hDirection > 0)
        {
            iTween.MoveUpdate(this.gameObject, iTween.Hash("x", transform.position.x + MOVE_LENGTH, "y", transform.position.y, "time", MOVE_TIME));
        }

        if(vDirection < 0)
        {
            iTween.MoveUpdate(this.gameObject, iTween.Hash("x", transform.position.x, "y", transform.position.y - MOVE_LENGTH, "time", MOVE_TIME));
        }
        else if (vDirection > 0)
        {
            iTween.MoveUpdate(this.gameObject, iTween.Hash("x", transform.position.x, "y", transform.position.y + MOVE_LENGTH, "time", MOVE_TIME));
        }
    }
}
