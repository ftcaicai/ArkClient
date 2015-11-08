using UnityEngine;
using System.Collections;

public class AsGuildNewListItem_Accept : MonoBehaviour 
{
	[SerializeField]SimpleSprite 	m_imgClassAman = null;
	[SerializeField]SimpleSprite 	m_imgClassDemigod = null;
	[SerializeField]SimpleSprite 	m_imgClassElf = null;
	[SerializeField]SimpleSprite 	m_imgClassLumicle = null;

	[SerializeField]SpriteText 		m_txtLevel = null;

	[SerializeField]SpriteText 		m_txtName = null;
	[SerializeField]SpriteText 		m_txtDate = null;
	[SerializeField]SpriteText 		m_txtConnection = null;

	[SerializeField]UIButton 			m_btnApprove = null;
	[SerializeField]UIButton 			m_btnRefuse = null;

	private body2_SC_GUILD_MEMBER_INFO_RESULT data = null;
	public body2_SC_GUILD_MEMBER_INFO_RESULT Data
	{
		get	{ return data; }
	}

	// Use this for initialization
	void Start () 
	{
		m_btnApprove.Text = AsTableManager.Instance.GetTbl_String (1152);
		m_btnRefuse.Text = AsTableManager.Instance.GetTbl_String (37920);

		m_btnApprove.SetInputDelegate( OnApproveBtn );
		m_btnRefuse.SetInputDelegate( OnRefuseBtn );
	}

	void OnApproveBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			body_CS_GUILD_SEARCH_JOIN_APPROVE joinApprove = new body_CS_GUILD_SEARCH_JOIN_APPROVE( data.nCharUniqKey, eGUILDJOINTYPE.eGUILDJOINTYPE_APPROVE);
			byte[] packet = joinApprove.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( packet);
		}
	}

	void OnRefuseBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			body_CS_GUILD_SEARCH_JOIN_APPROVE joinApprove = new body_CS_GUILD_SEARCH_JOIN_APPROVE( data.nCharUniqKey, eGUILDJOINTYPE.eGUILDJOINTYPE_NOT_APPROVE);
			byte[] packet = joinApprove.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( packet);
		}
	}


	// Update is called once per frame
	void Update () 
	{
	
	}

	public void Init( body2_SC_GUILD_MEMBER_INFO_RESULT data)
	{
		this.data = data;

		Color textColor = ( false == data.bConnect) ? Color.gray : Color.white;
		
		//	class , level , name
		m_imgClassAman.gameObject.SetActive(false);
		m_imgClassDemigod.gameObject.SetActive(false);
		m_imgClassElf.gameObject.SetActive(false);
		m_imgClassLumicle.gameObject.SetActive(false);
		switch( data.eClass)
		{
		case eCLASS.DIVINEKNIGHT:	m_imgClassDemigod.gameObject.SetActive(true);	break;
		case eCLASS.CLERIC:	m_imgClassLumicle.gameObject.SetActive(true);	break;
		case eCLASS.HUNTER:	m_imgClassAman.gameObject.SetActive(true);	break;
		case eCLASS.MAGICIAN:	m_imgClassElf.gameObject.SetActive(true);	break;
		default:	break;
		}
		
		m_txtLevel.Text = data.nLevel.ToString();
		m_txtName.Text = data.szCharName;
		m_txtDate.Text = "2015.04.23";		
		

		//	connection
		if( data.bConnect == false )
		{
			int	nOffDayCount = GetOffLineDay(data.nLastConnectTime);
			string strMsg = string.Format( AsTableManager.Instance.GetTbl_String(2038) , nOffDayCount );
			m_txtConnection.Text = strMsg;
			m_txtConnection.Color = textColor;
		}
		else
		{
			m_txtConnection.Text = "Online";
		}
	}

	private int  GetOffLineDay(long nTime)
	{		
		System.DateTime lastConnectTime  = new System.DateTime(1970, 1, 1, 9, 0, 0);
		lastConnectTime = lastConnectTime.AddSeconds(nTime);
		System.DateTime curTime = System.DateTime.Now;   
		
		System.TimeSpan span = curTime.Subtract(lastConnectTime);
		
		if(span.Days > 99) 
			return 99;
		
		return span.Days;
	}


}











































