using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum CameraMode
{
    FOLLOW_PLAYER,
    FROZEN
}

public class CameraController : MonoBehaviour {

    [SerializeField]
    CameraMode m_mode = CameraMode.FOLLOW_PLAYER;
    [SerializeField]
    Transform m_player;
    [SerializeField]
    float m_followSmoothTime = 0.3f;

    private Vector3 m_velocity = Vector3.zero;
    

	// Update is called once per frame
	void FixedUpdate ()
    {
		switch(m_mode)
        {
        case CameraMode.FOLLOW_PLAYER:
            FollowPlayerUpdate();
            break;
        case CameraMode.FROZEN:
            // Do nothing
            break;
        }

	}

    void FollowPlayerUpdate()
    {
        Vector3 targetPosition = m_player.TransformPoint(Vector3.zero);
        targetPosition.z = -10;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref m_velocity, m_followSmoothTime, 1000, Time.fixedDeltaTime);
    }
}
