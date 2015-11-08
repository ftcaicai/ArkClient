using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsPartyMemberMgr : MonoBehaviour
{
	public AsPartyMemberDlg[] m_PartyMemberDlgs = null;

	 // Awake
	void Awake()
	{
		DontDestroyOnLoad( gameObject);
		DDOL_Tracer.RegisterDDOL( this, gameObject);
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void SetCaptain( uint _id)
	{
		foreach( AsPartyMemberDlg memberDlg in m_PartyMemberDlgs)
		{
			memberDlg.SetCaptain( false);
			if ( _id !=0 && _id == memberDlg.CharUniqKey)
				memberDlg.SetCaptain( true);
		}
	}

	public void SetTarget( uint _id)
	{
		foreach( AsPartyMemberDlg memberDlg in m_PartyMemberDlgs)
		{
			memberDlg.SetTarget( false);
			if ( _id !=0 && _id == memberDlg.CharUniqKey)
				memberDlg.SetTarget( true, memberDlg.goChild.active);
		}
	}

	public void ClearPartyBuff()
	{
		foreach( AsPartyMemberDlg memberDlg in m_PartyMemberDlgs)
		{
			memberDlg.PartyBuffClear();
		}
	}

	public void ClosePartyMember()
	{
		foreach( AsPartyMemberDlg memberDlg in m_PartyMemberDlgs)
		{
			memberDlg.SetChildEnable(false);
		}
	}

	public void ReSetPartyMember()
	{
		ClearPartyBuff();
		ClosePartyMember();
		int order = 0;

		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null == userEntity)
		{
			Debug.Log( "PartyManager::SortPartyUser()GetCurrentUserEntity == null");//#20675
			return;
		}

		foreach( KeyValuePair<uint, AS_PARTY_USER> member in AsPartyManager.Instance.GetPartyMemberList())
		{
			if( member.Value.nCharUniqKey == userEntity.UniqueId)
				continue;

			AsPartyMemberDlg memberDlg = m_PartyMemberDlgs[order];
			memberDlg.PartyUserAdd( member.Value);
			memberDlg.Open();
			memberDlg.SetClassImage( member.Value.eClass-1);
			order++;

			if( member.Value.nSessionIdx == 0)//Zero is OffLine User
				memberDlg.SetOffLine( true);
			else
				memberDlg.SetOffLine( false);

			if( member.Value.isCaptain)
				SetCaptain( member.Value.nCharUniqKey);

			if( member.Value.m_BuffDataList != null)
			{
				for( int i = 0; i < member.Value.m_BuffDataList.Length; ++i)
					memberDlg.m_PartyBuffUI.BuffDataList.Add( member.Value.m_BuffDataList[i]);
				memberDlg.m_PartyBuffUI.SetShowUI();
			}

			memberDlg.SetCondition( member.Value.nCondition);//#20325.
		}
	}

	public AsPartyMemberDlg GetPartyMemberDlgByUniqueId( uint _id)
	{
		foreach( AsPartyMemberDlg memberDlg in m_PartyMemberDlgs)
		{
			if( memberDlg.CharUniqKey == _id)
				return memberDlg;
		}

		return null;
	}
}
