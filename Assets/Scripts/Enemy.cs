using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Animator animator;
    protected AudioSource explosion;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        explosion = GetComponent<AudioSource>();
    }

    public void JupmedOn()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<BoxCollider2D>().enabled = false;
        animator.SetTrigger("death");
        explosion.Play();
    }

    private void Death()
    {
        Destroy(this.gameObject);
    }
}
