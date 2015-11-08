using UnityEngine;
using System.Collections;

public class AsCharJump
{
	private Vector3 m_vec3TargetPos;
	private Vector3 m_vec3Value;
	private bool m_bJumping = false;

	public void StartJump( Vector3 vec3TargetPos)
	{
		m_vec3TargetPos = vec3TargetPos;
		m_bJumping = true;
	}

	public Vector3 GetValue()
	{
		return m_vec3Value;
	}

	public void Update( Vector3 vecCurPos, float fSpeed)
	{
		if( true == IsEndJump())
			return;

		Vector3 vec3Temp = GetMoveDelta( vecCurPos, m_vec3TargetPos, fSpeed);
		if( false == AsMath.Equals( vec3Temp.x, 0.0f) || false == AsMath.Equals( vec3Temp.z, 0.0f))
		{
			m_vec3Value = vec3Temp;
		}
		else
		{
			m_vec3Value = Vector3.zero;
			m_bJumping = false;
		}
	}

	public bool IsEndJump()
	{
		return !m_bJumping;
	}

	// move delta
	public Vector3 GetMoveDelta( Vector3 vec3CurPos, Vector3 vec3TargetPos, float fMaxSpeed, float tolerance = 0.000001f)
	{
		Vector3 vec3Movement = vec3TargetPos - vec3CurPos;
		if(vec3Movement.y <= 0.5f)
			vec3Movement.y = 0;

		if( fMaxSpeed < vec3Movement.magnitude)
		{
			vec3Movement.Normalize();
			vec3Movement *= fMaxSpeed;
		}
		else
			vec3Movement = Vector3.zero;

		return vec3Movement;
	}

}
