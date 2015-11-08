using UnityEngine;
using System.Collections;

public class AsCharMover 
{
	private Vector3[] m_path = null;
	private int m_iPathIndex = 0;
	private bool m_bMoving = false;
	private Vector3 m_vec3Movement = Vector3.zero;
//	private float m_fMoveTolerance = 0.000001f;
	
	public Vector3[] GetPath()
	{
		return m_path;
	}
	
	public bool IsMoving()
	{
		return m_bMoving;
	}
	
	public Vector3 GetMovement()
	{
		return m_vec3Movement;
	}
	
	public void SetPath( Vector3[] path ) 
	{
		m_path = path;
		m_iPathIndex = 0;
		m_bMoving = true;
		m_vec3Movement = Vector3.zero;
//		Debug.Log( " start path " );
	}
	
//	public void SetMoveTolerance( float ftolerance )
//	{
//		m_fMoveTolerance = ftolerance;
//	}
	
	public Vector3 GetCurPath()
	{
		if( null == m_path )
			return Vector3.zero;
		
		if( m_path.Length <= m_iPathIndex )
			return Vector3.zero;
			 
		return m_path[m_iPathIndex];
	}
	
	
	public void Update( Vector3 vec3CharPosition, float fMoveSpeed )
	{
		if( false == CheckPath() )
			return;		
		
		Vector3 vec3Movement = m_path[m_iPathIndex] - vec3CharPosition;
		//if(vec3Movement.y <= 5.5f)
			vec3Movement.y = 0;

		if( fMoveSpeed < vec3Movement.magnitude )
		{		 	
			m_vec3Movement = vec3Movement.normalized * fMoveSpeed;
		}
		else
		{
			m_vec3Movement = vec3Movement;	
			++ m_iPathIndex;
		}	
	}
	
		
	private bool CheckPath()
	{
		if( false == m_bMoving )
			return false;
		
		if( null == m_path || m_path.Length <= m_iPathIndex )
		{
			m_bMoving = false;
			m_vec3Movement = Vector3.zero;
			m_iPathIndex = 0;
			//Debug.Log( " End Move " );
		}
		
		return m_bMoving;
	}
		
	
	// move delta
	/*public Vector3 GetMoveDelta( Vector3 vec3CurPos, Vector3 vec3TargetPos, float fMaxSpeed, float tolerance = 0.000001f )
	{
		Vector3 vec3Movement = vec3TargetPos - vec3CurPos;
		if(vec3Movement.y <= 5.5f)
			vec3Movement.y = 0;

		if( fMaxSpeed < vec3Movement.magnitude )
		{
		 	vec3Movement.Normalize();
			vec3Movement *= fMaxSpeed;
		}
		else
		{
			
			vec3Movement = Vector3.zero;
		}
		
		return vec3Movement;
	}*/
		
	public Vector3 GetDestination()
	{
		return m_path[m_path.Length - 1];
	}
	
	public bool GetArrival(Vector3 _pos, float _tolerance)
	{
		Vector3 destinatioin = m_path[m_path.Length - 1];
		
		if((destinatioin - _pos).sqrMagnitude < _tolerance)
			return true;
		else
			return false;
	}
}
