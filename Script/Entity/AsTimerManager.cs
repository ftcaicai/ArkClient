using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void TimerFunc( System.Object _obj);

public class AsTimer{

	bool released_ = false;

	float time_;
	TimerFunc func_;
	System.Object obj_;

	float beginTime_;

	public AsTimer( float _time, TimerFunc _func, System.Object _obj)
	{
		time_ = _time;
		func_ = _func;
		obj_ = _obj;

		beginTime_ = Time.time;
	}

	public bool Update()
	{
		if( released_ == false)
		{
			if( time_ < Time.time - beginTime_)
			{
				func_( obj_);
				return released_ = true;
			}
			else
				return released_ = false;
		}
		else
			return true;
	}

	public void Release()
	{
		released_ = true;
	}

	public void Execution()
	{
		if( func_ != null)
			func_( obj_);

		released_ = true;
	}
}

public class AsTimerManager : MonoBehaviour
{
	#region - singleton -
	static AsTimerManager m_instance;
	public static AsTimerManager Instance	{ get { return m_instance; } }
	#endregion

	List<AsTimer> m_listTimer = new List<AsTimer>();

	void Awake()
	{
		m_instance = this;
	}

	void Update()
	{
		int i = 0;

//		Debug.Log( "AsTimerManager::Update: timer size is " + m_listTimer.Count);

		while( i < m_listTimer.Count)
		{
			if( true == m_listTimer[i].Update())
			{
				m_listTimer.RemoveAt( i);
				continue;
			}

			i++;
		}
	}

	public AsTimer SetTimer( float _time, TimerFunc _func)
	{
		return SetTimer( _time, _func, null);
	}

	public AsTimer SetTimer( float _time, TimerFunc _func, System.Object _obj)
	{
		AsTimer timer = new AsTimer( _time, _func, _obj);
		m_listTimer.Add( timer);

		return timer;
	}

	public void ReleaseTimer( AsTimer _timer)
	{
		_timer.Release();
	}

//	public void BeginTimer( 
//
//	IEnumerator TimerProcess()
//	{
//	}
}
