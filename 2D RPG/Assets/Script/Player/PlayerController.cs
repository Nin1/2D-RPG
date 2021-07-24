using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//@TODO: Move to a more general place
public enum Direction
{
    UP = 0,
    RIGHT,
    DOWN,
    LEFT
}

/** Handle Player Input */
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private bool m_useDigitalMovement = true;
    [SerializeField]
    private float m_walkSpeed = 0.1f;
    [SerializeField]
    private float m_runSpeed = 0.2f;
    [SerializeField]
    private float m_acceleration = 0.75f;
    [SerializeField]
    private float m_interactDistance = 1.5f;

    private Vector2 m_velocity = new Vector2();
    private Direction m_direction = Direction.UP;

    bool m_inputEnabled = true;

    void Start()
    {
        CutsceneRunner.instance.onCutsceneStart += DisableInput;
        CutsceneRunner.instance.onCutsceneEnd += EnableInput;
        Debug.Log("subscribed");
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (m_inputEnabled)
        {
            if (m_useDigitalMovement)
            {
                HandleDigitalMovement();
            }
            else
            {
                HandleAnalogueMovement();
            }
            if (InputManager.GetButtonPressed(InputButton.ACTION))
            {
                Interact();
            }
        }
	}

    private void HandleDigitalMovement()
    {
        Vector2 movementVector = new Vector2();

        if (InputManager.GetDirectionHeld(InputDirection.MAIN_UP))
        {
            movementVector.y = 1.0f;
        }
        if (InputManager.GetDirectionHeld(InputDirection.MAIN_DOWN))
        {
            movementVector.y = -1.0f;
        }
        if (InputManager.GetDirectionHeld(InputDirection.MAIN_LEFT))
        {
            movementVector.x = 1.0f;
        }
        if (InputManager.GetDirectionHeld(InputDirection.MAIN_RIGHT))
        {
            movementVector.x = -1.0f;
        }

        // Clamp movement vector so that diagonal movement isn't faster
        movementVector = Vector2.ClampMagnitude(movementVector, 1.0f);

        MovePlayer(movementVector);
    }

    private void HandleAnalogueMovement()
    {
        float horizontalAxis = InputManager.GetAxis(InputAxis.MAIN_HORIZONTAL);
        float verticalAxis = InputManager.GetAxis(InputAxis.MAIN_VERTICAL);

        // Clamp movement vector so that diagonal movement isn't faster
        Vector2 movementVector = new Vector2(horizontalAxis, verticalAxis);
        movementVector = Vector2.ClampMagnitude(movementVector, 1.0f);

        MovePlayer(movementVector);
    }

    private void MovePlayer(Vector2 movementVector)
    {
        float maxSpeed = 0.0f;
        if(InputManager.GetButtonHeld(InputButton.RUN))
        {
            maxSpeed = m_runSpeed;
        }
        else
        {
            maxSpeed = m_walkSpeed;
        }

        // Find target velocity
        Vector2 targetVelocity = movementVector * maxSpeed;
        Vector2 deltaVelocity = targetVelocity - m_velocity;

        {
            // Update velocity
            deltaVelocity.Normalize();
            // Update player velocity
            Vector2 accelleration = deltaVelocity * m_acceleration * Time.fixedDeltaTime;
            Vector2 oldVelocity = m_velocity;
            m_velocity = m_velocity + accelleration;

            // If velocity in a direction moves past 0, clamp it to 0.
            if ((oldVelocity.x > 0 && m_velocity.x < 0) || (oldVelocity.x < 0 && m_velocity.x > 0))
            {
                m_velocity.x = 0;
            }
            if ((oldVelocity.y > 0 && m_velocity.y < 0) || (oldVelocity.y < 0 && m_velocity.y > 0))
            {
                m_velocity.y = 0;
            }

            //m_velocity = Vector2.ClampMagnitude(m_velocity, m_maxSpeed);
        }

        // Move player
        //transform.Translate(m_velocity);
        Vector3 position = transform.position + (Vector3)m_velocity;
        GetComponent<Rigidbody2D>().MovePosition(position);

        UpdateMovementAnimation(movementVector.magnitude > 0.0f);
    }

    private void UpdateMovementAnimation(bool isWalking)
    {
        var animController = GetComponent<PlayerAnimController>();
        
        animController.SetWalking(isWalking);
        animController.SetVelocity(m_velocity);

        if (isWalking)
        {
            Direction dir = Direction.UP;
            // Get greatest magnitude from velocity x/y
            if (Mathf.Abs(m_velocity.x) > Mathf.Abs(m_velocity.y))
            {
                if (m_velocity.x > 0)
                {
                    dir = Direction.RIGHT;
                }
                else
                {
                    dir = Direction.LEFT;
                }
            }
            else
            {
                if (m_velocity.y > 0)
                {
                    dir = Direction.UP;
                }
                else
                {
                    dir = Direction.DOWN;
                }
            }
            animController.SetDirection(dir);
            m_direction = dir;
        }
    }

    /** Call OnInteract on the nearest interactible object in front of the player */
    private void Interact()
    {
        var hits = Physics2D.RaycastAll(transform.position, DirectionVector(m_direction), m_interactDistance);
        hits = hits.OrderBy(h => h.distance).ToArray();
        foreach(var hit in hits)
        {
            var collider = hit.collider;
            if(collider != null)
            {
                var interactScript = collider.transform.GetComponent<IInteractible>();
                if(interactScript != null)
                {
                    interactScript.OnInteract();
                    return;
                }
            }
        }
    }

    //@TODO: Move to a more general place alongside Direction enum
    public static Vector2 DirectionVector(Direction dir)
    {
        switch(dir)
        {
            case Direction.UP:
                return Vector2.up;
            case Direction.DOWN:
                return Vector2.down;
            case Direction.LEFT:
                return Vector2.left;
            case Direction.RIGHT:
                return Vector2.right;
        }
        Debug.LogError("Invalid direction given");
        return Vector2.zero;
    }

    void EnableInput()
    {
        m_inputEnabled = true;

    }

    void DisableInput()
    {
        m_inputEnabled = false;
        UpdateMovementAnimation(false);
    }
}
