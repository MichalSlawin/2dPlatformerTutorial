using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Animator animator;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void JupmedOn()
    {
        animator.SetTrigger("death");
    }

    private void Death()
    {
        Destroy(this.gameObject);
    }
}
