using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsPvpUserInfoDlg : MonoBehaviour
{
	private GameObject m_goRoot = null;
	public GameObject[] m_PvpUserInfo = new GameObject[8];
	private float m_fUserInfoUpdateTime = 0.0f;
	
	void Start()
	{
	}

	void Update()
	{
		float fCurTime = Time.realtimeSinceStartup;
		
		if( m_fUserInfoUpdateTime > 0 && ( fCurTime - m_fUserInfoUpdateTime) > 0.2f)
		{
			m_fUserInfoUpdateTime = fCurTime;
			
			_Update_PvpUserState();
		}

		if( AsUserInfo.Instance.SavedCharStat.hpCur_ <= 0)
			Close();
	}

	public void Open(GameObject goRoot)
	{
		gameObject.SetActiveRecursively( true);
		m_goRoot = goRoot;
		
		_Init();
		m_fUserInfoUpdateTime = Time.realtimeSinceStartup;
	}

	public void Close()
	{
		gameObject.SetActiveRecursively( false);

		if( null != m_goRoot)
			Destroy( m_goRoot);
	}
	
	#region -private
	private void _Init()
	{
		for( int i = 0; i < 8; i++)
			m_PvpUserInfo[i].gameObject.SetActiveRecursively( false);
		
		int nMyTeam = AsPvpManager.Instance.GetMyTeamType();
		int nIndex_l = 0;
		int nIndex_r = 4;
		
		foreach( KeyValuePair<uint, sARENAUSERINFO> pair in AsPvpManager.Instance.GetUserInfoList)
		{
			if( nMyTeam == pair.Value.nTeamType)
			{
				AsPvpUserInfo userInfo = m_PvpUserInfo[nIndex_l].GetComponent<AsPvpUserInfo>();
				m_PvpUserInfo[nIndex_l].SetActiveRecursively( true);
				userInfo.SetUserInfo( pair.Value, true);
				nIndex_l++;
			}
			else
			{
				AsPvpUserInfo userInfo = m_PvpUserInfo[nIndex_r].GetComponent<AsPvpUserInfo>();
				m_PvpUserInfo[nIndex_r].SetActiveRecursively( true);
				userInfo.SetUserInfo( pair.Value, false);
				nIndex_r++;
			}
		}
		
		/*
		//  7.7  |  5.1
		//  2.55 |  0
		// -2.55 | -5.1
		// -7.7
		float fZ = -10.0f;
		float fX = -500.0f;
		int nUserInfoCount = AsPvpManager.Instance.GetUserInfoList.Count;
		if( 2 == nUserInfoCount)
		{
			m_PvpUserInfo[0].gameObject.transform.position = new Vector3( -8.3f + fX, 0.0f, fZ);
			m_PvpUserInfo[4].gameObject.transform.position = new Vector3( 8.3f + fX, 0.0f, fZ);
		}
		else if( 4 == nUserInfoCount)
		{
			m_PvpUserInfo[0].gameObject.transform.position = new Vector3( -8.3f + fX, 2.2f, fZ);
			m_PvpUserInfo[1].gameObject.transform.position = new Vector3( -8.3f + fX, -2.2f, fZ);
			m_PvpUserInfo[4].gameObject.transform.position = new Vector3( 8.3f + fX, 2.2f, fZ);
			m_PvpUserInfo[5].gameObject.transform.position = new Vector3( 8.3f + fX, -2.2f, fZ);
		}
		else if( 6 == nUserInfoCount)
		{
			m_PvpUserInfo[0].gameObject.transform.position = new Vector3( -8.3f + fX, 4.4f, fZ);
			m_PvpUserInfo[1].gameObject.transform.position = new Vector3( -8.3f + fX, 0.0f, fZ);
			m_PvpUserInfo[2].gameObject.transform.position = new Vector3( -8.3f + fX, -4.4f, fZ);
			m_PvpUserInfo[4].gameObject.transform.position = new Vector3( 8.3f + fX, 4.4f, fZ);
			m_PvpUserInfo[5].gameObject.transform.position = new Vector3( 8.3f + fX, 0.0f, fZ);
			m_PvpUserInfo[6].gameObject.transform.position = new Vector3( 8.3f + fX, -4.4f, fZ);
		}
		else if( 8 == nUserInfoCount)
		{
			m_PvpUserInfo[0].gameObject.transform.position = new Vector3( -8.3f + fX, 6.6f, fZ);
			m_PvpUserInfo[1].gameObject.transform.position = new Vector3( -8.3f + fX, 2.2f, fZ);
			m_PvpUserInfo[2].gameObject.transform.position = new Vector3( -8.3f + fX, -2.2f, fZ);
			m_PvpUserInfo[3].gameObject.transform.position = new Vector3( -8.3f + fX, -6.6f, fZ);
			m_PvpUserInfo[4].gameObject.transform.position = new Vector3( 8.3f + fX, 6.6f, fZ);
			m_PvpUserInfo[5].gameObject.transform.position = new Vector3( 8.3f + fX, 2.2f, fZ);
			m_PvpUserInfo[6].gameObject.transform.position = new Vector3( 8.3f + fX, -2.2f, fZ);
			m_PvpUserInfo[7].gameObject.transform.position = new Vector3( 8.3f + fX, -6.6f, fZ);
		}
		*/
		float fZ = -10.0f;
		float fX = 0.0f;
		int nUserInfoCount = AsPvpManager.Instance.GetUserInfoList.Count;
		if( 2 == nUserInfoCount)
		{
			m_PvpUserInfo[0].gameObject.transform.localPosition = new Vector3( fX, 0.0f, fZ);
			m_PvpUserInfo[4].gameObject.transform.localPosition = new Vector3( fX, 0.0f, fZ);
		}
		else if( 4 == nUserInfoCount)
		{
			m_PvpUserInfo[0].gameObject.transform.localPosition = new Vector3( fX, 2.2f, fZ);
			m_PvpUserInfo[1].gameObject.transform.localPosition = new Vector3( fX, -2.2f, fZ);
			m_PvpUserInfo[4].gameObject.transform.localPosition = new Vector3( fX, 2.2f, fZ);
			m_PvpUserInfo[5].gameObject.transform.localPosition = new Vector3( fX, -2.2f, fZ);
		}
		else if( 6 == nUserInfoCount)
		{
			m_PvpUserInfo[0].gameObject.transform.localPosition = new Vector3( fX, 4.4f, fZ);
			m_PvpUserInfo[1].gameObject.transform.localPosition = new Vector3( fX, 0.0f, fZ);
			m_PvpUserInfo[2].gameObject.transform.localPosition = new Vector3( fX, -4.4f, fZ);
			m_PvpUserInfo[4].gameObject.transform.localPosition = new Vector3( fX, 4.4f, fZ);
			m_PvpUserInfo[5].gameObject.transform.localPosition = new Vector3( fX, 0.0f, fZ);
			m_PvpUserInfo[6].gameObject.transform.localPosition = new Vector3( fX, -4.4f, fZ);
		}
		else if( 8 == nUserInfoCount)
		{
			m_PvpUserInfo[0].gameObject.transform.localPosition = new Vector3( fX, 6.6f, fZ);
			m_PvpUserInfo[1].gameObject.transform.localPosition = new Vector3( fX, 2.2f, fZ);
			m_PvpUserInfo[2].gameObject.transform.localPosition = new Vector3( fX, -2.2f, fZ);
			m_PvpUserInfo[3].gameObject.transform.localPosition = new Vector3( fX, -6.6f, fZ);
			m_PvpUserInfo[4].gameObject.transform.localPosition = new Vector3( fX, 6.6f, fZ);
			m_PvpUserInfo[5].gameObject.transform.localPosition = new Vector3( fX, 2.2f, fZ);
			m_PvpUserInfo[6].gameObject.transform.localPosition = new Vector3( fX, -2.2f, fZ);
			m_PvpUserInfo[7].gameObject.transform.localPosition = new Vector3( fX, -6.6f, fZ);
		}
	}
	
	private void _Update_PvpUserState()
	{
		foreach( KeyValuePair<uint, sARENAUSERINFO> pair in AsPvpManager.Instance.GetUserInfoList)
		{
			_SetPvpUserState( pair.Value.nCharUniqKey, pair.Value.nEnterState);
		}
	}
	
	private void _SetPvpUserState(uint nCharUniqKey, int nEnterState)
	{
		for( int i = 0; i < 8; i++)
		{
			if( true == m_PvpUserInfo[i].gameObject.active)
			{
				AsPvpUserInfo userInfo = m_PvpUserInfo[i].GetComponent<AsPvpUserInfo>();

				if( userInfo.GetCharUniqKey() == nCharUniqKey)
				{
					userInfo.SetUserState( nEnterState);
					return;
				}
			}
		}
	}
	#endregion
}
