using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputAxis
{
    MAIN_HORIZONTAL,
    MAIN_VERTICAL
}

public enum InputDirection
{
    MAIN_UP,
    MAIN_DOWN,
    MAIN_LEFT,
    MAIN_RIGHT
}

public enum InputButton
{
    ACTION,
    CANCEL,
    RUN,
}

public enum ButtonState
{
    UP,
    DOWN,
    DOWN_THIS_FRAME,
    UP_THIS_FRAME,
}

public class InputManager : MonoBehaviour {
    
    /** The threshhold after which an "axis" becomes "pressed" */
    private const float DIRECTION_THRESHOLD = 0.2f;

    /** Map of what directions are currently pressed down */
    private static Dictionary<InputDirection, ButtonState> m_digitalDirections;

    public void Awake()
    {

        // Populate m_digitalDirections
        m_digitalDirections = new Dictionary<InputDirection, ButtonState>();
        foreach (InputDirection dir in System.Enum.GetValues(typeof(InputDirection)))
        {
            m_digitalDirections[dir] = ButtonState.UP;
        }
    }

    public void FixedUpdate()
    {
        UpdateDigitalDirections();
    }

    //###########
    //# Buttons #
    //###########

    private static string GetButtonStringFromEnum(InputButton button)
    {
        switch(button)
        {
            case InputButton.ACTION:
                return "Action";
            case InputButton.CANCEL:
                return "Cancel";
            case InputButton.RUN:
                return "Run";
        }
        Debug.LogError("Button missing: " + button);
        return "Error";
    }

    public static bool GetButtonPressed(InputButton button)
    {
        return Input.GetButtonDown(GetButtonStringFromEnum(button));
    }

    public static bool GetButtonHeld(InputButton button)
    {
        return Input.GetButton(GetButtonStringFromEnum(button));
    }

    public static bool GetButtonReleased(InputButton button)
    {
        return Input.GetButtonUp(GetButtonStringFromEnum(button));
    }

    //##############
    //# Directions #
    //##############

    // Public functions

    public static float GetAxis(InputAxis axis)
    {
        switch (axis)
        {
            case InputAxis.MAIN_HORIZONTAL:
                return Input.GetAxisRaw("Horizontal");
            case InputAxis.MAIN_VERTICAL:
                return Input.GetAxisRaw("Vertical");
            default:
                return 0.0f;
        }
    }

    public static bool GetDirectionPressed(InputDirection dir)
    {
        return m_digitalDirections[dir] == ButtonState.DOWN_THIS_FRAME;
    }

    public static bool GetDirectionHeld(InputDirection dir)
    {
        return m_digitalDirections[dir] == ButtonState.DOWN || m_digitalDirections[dir] == ButtonState.DOWN_THIS_FRAME;
    }

    public static bool GetDirectionReleased(InputDirection dir)
    {
        return m_digitalDirections[dir] == ButtonState.UP_THIS_FRAME;
    }

    // Private functions

    /** @return whether the given direction is pressed or not */
    private static bool IsDirectionPressed(InputDirection dir)
    {
        switch (dir)
        {
            case InputDirection.MAIN_UP:
                return GetAxis(InputAxis.MAIN_VERTICAL) > DIRECTION_THRESHOLD;
            case InputDirection.MAIN_DOWN:
                return GetAxis(InputAxis.MAIN_VERTICAL) < -DIRECTION_THRESHOLD;
            case InputDirection.MAIN_LEFT:
                return GetAxis(InputAxis.MAIN_HORIZONTAL) > DIRECTION_THRESHOLD;
            case InputDirection.MAIN_RIGHT:
                return GetAxis(InputAxis.MAIN_HORIZONTAL) < -DIRECTION_THRESHOLD;
        }
        return false;
    }

    /** Called by Update() to update the ButtonState of each direction */
    private static void UpdateDigitalDirections()
    {
        List<InputDirection> directions = new List<InputDirection>(m_digitalDirections.Keys);

        foreach(InputDirection dir in directions)
        {
            // Get the state of the directional button this frame
            bool directionPressed = IsDirectionPressed(dir);
            ButtonState oldState = m_digitalDirections[dir];

            // Update the stored state of the directional button
            if (directionPressed)
            {
                if (oldState == ButtonState.UP || oldState == ButtonState.UP_THIS_FRAME)
                {
                    // The button was pressed this frame
                    m_digitalDirections[dir] = ButtonState.DOWN_THIS_FRAME;
                }
                else
                {
                    m_digitalDirections[dir] = ButtonState.DOWN;
                }
            }
            else
            {
                if (oldState == ButtonState.DOWN || oldState == ButtonState.DOWN_THIS_FRAME)
                {
                    // The button was released this frame
                    m_digitalDirections[dir] = ButtonState.UP_THIS_FRAME;
                }
                else
                {
                    m_digitalDirections[dir] = ButtonState.UP;
                }
            }
        }
    }
}
