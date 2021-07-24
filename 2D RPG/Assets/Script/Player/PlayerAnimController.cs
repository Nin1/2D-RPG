using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour {

    Animator m_animator;

    void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    public void SetDirection(Direction dir)
    {
        m_animator.SetInteger("Direction", (int)dir);
    }

    public void SetWalking(bool isWalking)
    {
        m_animator.SetBool("Walking", isWalking);
    }

    public void SetVelocity(Vector2 velocity)
    {
        m_animator.SetFloat("XSpeed", velocity.x);
        m_animator.SetFloat("YSpeed", velocity.y);
    }
}
