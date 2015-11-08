using UnityEngine;
using System.Collections;
using System.Text;

public class AsEventNotifyMgr : MonoBehaviour
{
	#region - singleton -
	static AsEventNotifyMgr m_instance;
	public static AsEventNotifyMgr Instance	{ get { return m_instance; } }
	#endregion

	public Color m_NameColor;
	public Color m_GetTypeColor;
	public Color m_GMMssageColor;
	public Color m_ItemPutOn;
	public Color m_Strengthen;
	public Color m_StrengthenNum;
	public Color m_ResetAlarmColor;
	public Color m_GuildNoticeColor;

	AsLevelUpNotify m_LevelUpNotify = null;
	public AsLevelUpNotify LevelUpNotify
	{
		get { return m_LevelUpNotify; }
	}

	AsCenterNotify m_CenterNotify = null;
	public AsCenterNotify CenterNotify
	{
		get { return m_CenterNotify; }
	}

	AsItemGetNotify m_ItemGetNotify = null;
	public AsItemGetNotify ItemGetNotify
	{
		get{ return m_ItemGetNotify; }
	}
	
	AsItemGetAlarmNotify m_ItemGetAlarmNotify = null;
	public AsItemGetAlarmNotify ItemGetAlarmNotify
	{
		get{ return m_ItemGetAlarmNotify; }
	}

	body_SC_USER_EVENT_NOTIFY m_randItemNotify;
	public void SetRandItemNotify( body_SC_USER_EVENT_NOTIFY _data)
	{
		m_randItemNotify = _data;
	}

	public void PlayRandItemNotify()
	{
		if( null == m_randItemNotify)
			return;

		ReceiveUserEventNotify( m_randItemNotify);
		m_randItemNotify = null;
	}

	void Awake()
	{
		#region - singleton -
		m_instance = this;
		#endregion

		m_LevelUpNotify = gameObject.AddComponent<AsLevelUpNotify>();
		m_CenterNotify = gameObject.AddComponent<AsCenterNotify>();
		m_ItemGetNotify = gameObject.AddComponent<AsItemGetNotify>();
		m_ItemGetAlarmNotify = gameObject.AddComponent<AsItemGetAlarmNotify>();
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		m_ItemGetNotify.UpdateData();
		m_CenterNotify.UpdateData();
		ItemGetAlarmNotify.UpdateData();
	}

	public void ReceiveUserEventNotify( body_SC_USER_EVENT_NOTIFY notify)
	{
		
		if( (eUSEREVENTTYPE)notify.eType == eUSEREVENTTYPE.eUSEREVENTTYPE_ITEM)
		{
			Item item = ItemMgr.ItemManagement.GetItem( notify.sItem.nItemTableIdx);
			if( null == item)
				return;

			if( notify.nValue_1 == (int)eUSEREVENT_ITEM_GETTYPE.eUSEREVENT_ITEM_GETTYPE_STRENGTHEN)
			{
				CenterNotify.AddItemStrengthenOnMessage( notify);
				return;
			}

			StringBuilder sbItemName = new StringBuilder();
			sbItemName.Insert( 0, item.ItemData.GetGradeColor().ToString());
			sbItemName.AppendFormat( "{0}", AsTableManager.Instance. GetTbl_String( item.ItemData.nameId));
			sbItemName.AppendFormat( "{0}", AsChatManager.Instance.GetChatTypeColor( eCHATTYPE.eCHATTYPE_SYSTEM_ITEM));

			StringBuilder getType = new StringBuilder();

			switch( (eUSEREVENT_ITEM_GETTYPE)notify.nValue_1)
			{
				case eUSEREVENT_ITEM_GETTYPE.eUSEREVENT_ITEM_GETTYPE_HUNTING:
					getType.Insert( 0, AsTableManager.Instance. GetTbl_String(842));
					break;
				case eUSEREVENT_ITEM_GETTYPE.eUSEREVENT_ITEM_GETTYPE_MIX:
					getType.Insert( 0, AsTableManager.Instance. GetTbl_String(843));
					break;
				case eUSEREVENT_ITEM_GETTYPE.eUSEREVENT_ITEM_GETTYPE_RULLET:
				case eUSEREVENT_ITEM_GETTYPE.eUSEREVENT_ITEM_GETTYPE_RANDOMBOX:
					getType.Insert( 0, AsTableManager.Instance. GetTbl_String(844));
					break;
				case eUSEREVENT_ITEM_GETTYPE.eUSEREVENT_ITEM_GETTYPE_COLLECTING:
					getType.Insert( 0, AsTableManager.Instance. GetTbl_String(845));
					break;
				case eUSEREVENT_ITEM_GETTYPE.eUSEREVENT_ITEM_GETTYPE_PRODUCT:
					getType.Insert( 0, AsTableManager.Instance. GetTbl_String(846));
					break;
			}

			bool isEquipItem = Item.eITEM_TYPE.EquipItem == item.ItemData.GetItemType() || Item.eITEM_TYPE.CosEquipItem == item.ItemData.GetItemType();
			//-->머리위 표시는 매직등급 이상 획득 시 표시.
			//-->채팅창 외치기는 에픽등급 이상 획득 시 표시.
			if( item.ItemData.grade >= Item.eGRADE.Epic && isEquipItem)
			{ //시스템 메세지 표시.RGBA( 0.0,0.7,0.0,1.0){0}RGBA( 1.0,0.4,0.6,1.0)님이 RGBA( 0.9,0.0,0.0,1.0){1}RGBA( 1.0,0.4,0.6,1.0)을( 를) 통해 [{2}RGBA( 1.0,0.4,0.6,1.0)]을( 를) 획득하였습니다!!!
				string msg = string.Format( AsTableManager.Instance. GetTbl_String(836),AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( notify.szCharName)),
					getType.ToString(), sbItemName.ToString());
				AsChatManager.Instance.InsertSystemChat( msg, eCHATTYPE.eCHATTYPE_SYSTEM_ITEM);

				AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
				if( null != userEntity)
				{
					string userName = userEntity.GetProperty<string>( eComponentProperty.NAME);
					if( userName.CompareTo( AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( notify.szCharName))) != 0)
					{
						CenterNotify.AddItemGetMessage( msg);
					}
				}
			}

		/*	if( isEquipItem && item.ItemData.grade >= Item.eGRADE.Magic && (eUSEREVENT_ITEM_GETTYPE)notify.nValue_1 != eUSEREVENT_ITEM_GETTYPE.eUSEREVENT_ITEM_GETTYPE_RULLET)
			{
				AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
				if( null != userEntity)
				{
					string userName = userEntity.GetProperty<string>( eComponentProperty.NAME);
					if( userName.CompareTo( AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( notify.szCharName))) == 0)
						m_ItemGetNotify.DisplayItemPanel( notify.sItem.nItemTableIdx);
				}
			}*/
			
			if( isEquipItem && item.ItemData.grade >= Item.eGRADE.Rare )
			{
				AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
				AsUserEntity entityEntity = AsEntityManager.Instance.GetUserEntityByUniqueId( (uint)notify.nValue_2 );
				if( null != userEntity && null != entityEntity )
				{					
					//if( true == entityEntity.IsCheckEquipEnable(item) )
					//{
						if( userEntity == entityEntity )
						{							
							ItemGetAlarmNotify.AddListItem( notify.sItem );
						}
						else
						{
							entityEntity.ShowItemGetAlarmBallonImg( notify.sItem, true );
						}
					//}
				}
			}
			
		}
	}

    public void ShowGetItemAlrameBalloon(sITEM _sItem)
    {
        Item item = ItemMgr.ItemManagement.GetItem(_sItem.nItemTableIdx);

        if (null == item)
            return;

        if (item.ItemData.grade < Item.eGRADE.Rare)
            return;

        ItemGetAlarmNotify.AddListItem(_sItem);
    }
}
