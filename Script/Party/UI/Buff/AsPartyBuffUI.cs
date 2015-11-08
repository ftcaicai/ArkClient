using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AsPartyBuffUI : BuffBaseMgr
{
	private List<PlayerBuffData> m_BuffDataList = new List<PlayerBuffData>();
	private bool m_isReset = false;

	public List<PlayerBuffData> BuffDataList
	{
		get { return m_BuffDataList;}
		set { m_BuffDataList = value;}
	}

	protected void ResetUiBuffSlot()
	{
		if( null == uiBuffSlots)
		{
			Debug.LogError( "null == m_BuffSlots");
			return;
		}

		for ( int i = 0; i < uiBuffSlots.Length; ++i)
		{
			if( null == uiBuffSlots[i])
			{
				AsUtil.ShutDown( "UIBuffSlots is null ");
				continue;
			}

			if ( i < m_BuffDataList.Count)
				uiBuffSlots[i].OpenBuffSlot( m_BuffDataList[i]);
			else
				uiBuffSlots[i].OffBuffSlot();
		}

		if( uiBuffSlots.Length < m_BuffDataList.Count)
			SetActiveEtc( true);
		else
			SetActiveEtc( false);
	}

	public void InsertBuffData( body1_SC_CHAR_BUFF _data)
	{
		foreach( body2_SC_CHAR_BUFF buff in _data.body)
		{
			if( 0 == buff.nDuration)
				continue;

			if( true == IsMonsterSkillIndex( buff.nSkillTableIdx))
			{
				if( false == IsCheckHaveMobSkillIcon( buff.nSkillTableIdx, buff.nPotencyIdx))
					continue;
			}
			else
			{
				if( false == IsCheckHaveIcon( buff.nSkillTableIdx, buff.nPotencyIdx))
					continue;
			}

			m_BuffDataList.Add( new PlayerBuffData( buff));
		}
	}

	protected void DeleteBuffData( body_SC_CHAR_DEBUFF data)
	{
		foreach( PlayerBuffData buffdata in m_BuffDataList)
		{
			if( buffdata.GetSkillIdx() == data.nSkillTableIdx && buffdata.GetPotencyIdx() == data.nPotencyIdx)
			{
				m_BuffDataList.Remove( buffdata);
				return;
			}
		}
	}

	public void ReciveBuff( body1_SC_CHAR_BUFF _data, bool _isReset = true )
	{
		m_isReset = true;
		InsertBuffData( _data);
		if( true == _isReset)
			ResetUiBuffSlot();
	}

	public void DeleteBuff( body_SC_CHAR_DEBUFF _data, bool _isReset = true)
	{
		m_isReset = true;
		DeleteBuffData( _data);
		if( true == _isReset )
			ResetUiBuffSlot();
	}

	//$yde
	public List<body2_SC_CHAR_BUFF> CharBuffDataList
	{
		get
		{
			List<body2_SC_CHAR_BUFF> list = new List<body2_SC_CHAR_BUFF>();
			foreach( PlayerBuffData data in m_BuffDataList)
			{
				list.Add( data.getServerData);
			}

			return list;
		}
	}

	public void SetShowUI()
	{
		ResetUiBuffSlot();
	}

	public void Clear()
	{
		m_BuffDataList.Clear();
		ResetUiBuffSlot();
	}

	void Awake()
	{
		SetActiveEtc( false);
	}

	public override void UpdateLogic()
	{
		base.UpdateLogic();

		if( m_isReset)
		{
			m_isReset = false;
			ResetUiBuffSlot();
		}

		for ( int i = 0; i < m_BuffDataList.Count; i++)
		{
			PlayerBuffData buffdata = m_BuffDataList[i];
			buffdata.Update();
		}
	}
}
