using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CharacterChangeController : MonoBehaviour
{
    private const KeyCode CHANGE_CHARACTER = KeyCode.R;

    private bool isPlayer = true;
    private Renderer playerRenderer;
    private Renderer eagleRenderer;
    private Rigidbody2D eagleRB;
    private Rigidbody2D playerRB;
    private BoxCollider2D eagleCollider;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject eagle;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
 

    // Start is called before the first frame update
    void Start()
    {
        playerRenderer = player.GetComponent<Renderer>();
        eagleRenderer = eagle.GetComponent<Renderer>();
        eagleRenderer.enabled = false;
        eagleRB = eagle.GetComponent<Rigidbody2D>();
        playerRB = player.GetComponent<Rigidbody2D>();
        eagleCollider = eagle.GetComponent<BoxCollider2D>();
        eagleCollider.enabled = false;
    }

    public void SwitchCharacter()
    {
        isPlayer = !isPlayer;

        if(isPlayer)
        {
            eagleRenderer.enabled = false;
            eagleRB.bodyType = RigidbodyType2D.Static;
            eagleCollider.enabled = false;
            //eagle.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

            playerRB.bodyType = RigidbodyType2D.Dynamic;
            cinemachineVirtualCamera.m_Follow = player.transform;
        }
        else
        {
            eagleRenderer.enabled = true;
            eagleRB.bodyType = RigidbodyType2D.Kinematic;
            eagleCollider.enabled = true;
            //eagle.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

            playerRB.bodyType = RigidbodyType2D.Static;
            cinemachineVirtualCamera.m_Follow = eagle.transform;
        }
    }
}
